using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public byte[]? AvatarImage { get; set; }
        public string? TelegramID { get; set; }
        public string? ChatId { get; set; }
        public bool AdminUser { get; set; }
        public NotificationSettingsModel? NotificationSettings { get; set; }
    }
}
