using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;
using Telegram.Bot;

namespace TaskManager.Services
{
    public class NotificationTGService : INotificationService
    {
        #region Properties
        private readonly ITelegramBotClient _botClient;
        private readonly INotificationSettingsService _settingsService;
        #endregion

        #region Initialiation
        public NotificationTGService(ITelegramBotClient botClient, INotificationSettingsService settingsService)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(_botClient));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }
        #endregion

        #region Methods
        public async Task SendNotification(ApplicationUser user, string message, 
            NotificationTypeModel notificationType = NotificationTypeModel.General)
        {
            try
            {
                if (string.IsNullOrEmpty(user.ChatId))
                {
                    Console.WriteLine($"{user.ChatId} is not found");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync(user);
                bool shouldSend = false;

                switch (notificationType)
                {
                    case NotificationTypeModel.TaskAdded:
                        shouldSend = settings.NotifyOnTaskAdded;
                        break;
                    case NotificationTypeModel.TaskChanged:
                        shouldSend = settings.NotifyOnTaskChanged;
                        break;
                    case NotificationTypeModel.TaskDeleted:
                        shouldSend = settings.NotifyOnTaskDeleted;
                        break;
                    case NotificationTypeModel.TaskReminder:
                        shouldSend = settings.NotifyOnTaskReminder;
                        break;
                    case NotificationTypeModel.TaskCompleted:
                        shouldSend = settings.NotifyOnTaskCompleted;
                        break;
                    default:
                        shouldSend = true;
                        break;
                }

                if (!shouldSend)
                {
                    return;
                }

                await _botClient.SendMessage(
                    chatId: user.ChatId,
                    text: message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex}");
            }
        }
        #endregion
    }
}