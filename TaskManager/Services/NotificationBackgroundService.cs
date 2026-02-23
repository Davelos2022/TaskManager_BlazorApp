using Microsoft.Extensions.Options;
using System.Collections.Generic;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        #region Properties
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NotificationOptions _opts;

        private TimeSpan _reminderLead;
        private TimeSpan _resendInterval;
        private TimeSpan _checkInterval;
        #endregion

        #region Initialiation
        public NotificationBackgroundService(IServiceScopeFactory scopeFactory,IOptions<NotificationOptions> opts)
        {
            _scopeFactory = scopeFactory;
            _opts = opts.Value;
        }
        #endregion

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _reminderLead = TimeSpan.FromHours(_opts.ReminderLeadHours);
            _resendInterval = TimeSpan.FromMinutes(_opts.ResendIntervalMinutes);
            _checkInterval = TimeSpan.FromMinutes(_opts.CheckIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckDeadlinesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error {ex}");
                }

                try { await Task.Delay(_checkInterval, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }

        private async Task CheckDeadlinesAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ITaskRepository taskRepo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            INotificationService notificationSv = scope.ServiceProvider.GetRequiredService<INotificationService>();
            INotificationSettingsService settingsSv = scope.ServiceProvider.GetRequiredService<INotificationSettingsService>();
            IUtilService utilSv = scope.ServiceProvider.GetRequiredService<IUtilService>();

            DateTime nowUtc = DateTime.UtcNow;
            IEnumerable<TaskModel> tasksToNotify = await taskRepo.GetDueTasksAsync(nowUtc, _reminderLead);

            if (tasksToNotify == null) return;

            int startHour = _opts.StartHour;
            int endHour = _opts.EndHour;

            foreach (var task in tasksToNotify)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                ApplicationUser user = task.User;
                if (user == null) continue;

                NotificationSettingsModel userSettings = await settingsSv.GetSettingsAsync(user);
                int offsetMin = userSettings.TimeZoneOffsetMinutes;
                DateTime nowLocal = nowUtc.AddMinutes(-offsetMin);

                bool withinLocalWindow = (startHour <= endHour)
                    ? (nowLocal.Hour >= startHour && nowLocal.Hour <= endHour)
                    : (nowLocal.Hour >= startHour || nowLocal.Hour <= endHour);

                if (!withinLocalWindow) continue;

                try
                {
                    if (task.LastReminderSent == null ||
                       (nowUtc - task.LastReminderSent.Value) > _resendInterval)
                    {
                        string template = nowUtc <= task.DueDate
                            ? ApplicationConstants.NOTIFICATION_TASK_DEAD_LINE
                            : ApplicationConstants.NOTIFICATION_TASK_OVERDUE;

                        string message = string.Format(
                            template,
                            task.Title,
                            utilSv.FormatDate(task.DueDate.Value)
                        );

                        await notificationSv.SendNotification(user, message);
                        task.LastReminderSent = nowUtc;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to notify task {task.Id}: {ex}");
                }
            }
        }
        #endregion
    }
}