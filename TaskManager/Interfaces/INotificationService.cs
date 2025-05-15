using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface INotificationService
    {
        Task SendNotification(ApplicationUser user, string message, NotificationTypeModel notificationType = NotificationTypeModel.General);
    }
}