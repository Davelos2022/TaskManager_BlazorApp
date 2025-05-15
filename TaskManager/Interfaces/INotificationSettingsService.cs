using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface INotificationSettingsService
    {
        Task<NotificationSettingsModel> GetSettingsAsync(ApplicationUser user);
        Task SaveSettingsAsync(NotificationSettingsModel settings);
    }
}