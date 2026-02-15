using TaskManager.Data;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        #region Properties
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _reminderLead = TimeSpan.FromHours(ApplicationConstants.NOTICE_COUNTDOWN_LEAD_HOURS);
        private readonly TimeSpan _resendInterval = TimeSpan.FromMinutes(ApplicationConstants.NOTIFICATION_RESEND_INTERVAL_MINUTES);
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(ApplicationConstants.NOTIFICATION_CHECK_INTERVAL_MINUTES);
        #endregion

        #region Initialiation
        public NotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }
        #endregion

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
            using var scope = _scopeFactory.CreateScope();
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var utilService = scope.ServiceProvider.GetRequiredService<IUtilService>();

            var now = DateTime.UtcNow;
            var tasksToNotify = await taskRepository.GetDueTasksAsync(now, _reminderLead);
            if (tasksToNotify == null) return;

            var start = ApplicationConstants.NOTIFICATION_START_HOUR;
            var end = ApplicationConstants.NOTIFICATION_END_HOUR;

            bool withinWindow = (start <= end) ? (now.Hour >= start && now.Hour <= end) : (now.Hour >= start || now.Hour <= end);

            if (withinWindow)
            {
                foreach (var task in tasksToNotify)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    if (task?.User == null) continue;

                    try
                    {
                        if (task.LastReminderSent == null || (now - task.LastReminderSent.Value) > _resendInterval)
                        {
                            string message = string.Format(
                                now <= task.DueDate ? ApplicationConstants.NOTIFICATION_TASK_DEAD_LINE : ApplicationConstants.NOTIFICATION_TASK_OVERDUE,
                                task.Title,
                                utilService.FormatDate(task.DueDate.Value)
                            );

                            await notificationService.SendNotification(task.User, message);
                            task.LastReminderSent = now;
                        }
                    }
                    catch (Exception exTask)
                    {
                        Console.WriteLine($"Failed to notify task {task?.Id}: {exTask}");
                    }
                }
            }
        }
        #endregion
    }
}