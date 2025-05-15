using TaskManager.Data;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        #region Properties
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(ApplicationConstants.INTERVAL_MINUTES);
        private readonly TimeSpan _reminderThreshold = TimeSpan.FromHours(ApplicationConstants.INTERVAL_HOUSE);
        #endregion

        #region Initialiation
        public NotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(_scopeFactory));
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

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckDeadlinesAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var utilService = scope.ServiceProvider.GetRequiredService<IUtilService>();
                   
                var now = DateTime.UtcNow;
                var tasksToNotify = await taskRepository.GetDueTasksAsync(now, _reminderThreshold);

                if (now.Hour <= ApplicationConstants.MAX_HOUSE_NOTIFICATION && 
                    now.Hour >= ApplicationConstants.MIN_HOUSE_NOTIFICATION)
                {
                    foreach (var task in tasksToNotify)
                    {
                        if (task.User != null)
                        {
                            if (task.LastReminderSent == null || (now - task.LastReminderSent.Value) > _reminderThreshold)
                            {
                                string message = string.Format(ApplicationConstants.NOTIFICATION_TASK_DEAD_LINE, task.Title, utilService.FormatDate(task.DueDate.Value));
                                await notificationService.SendNotification(task.User, message);
                                task.LastReminderSent = now;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}