using System.ComponentModel.DataAnnotations;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class NotificationSettingsModel
    {
        [Key]
        public string UserId { get; set; }

        public bool NotifyOnTaskAdded { get; set; }
        public bool NotifyOnTaskChanged { get; set; }
        public bool NotifyOnTaskDeleted { get; set; }
        public bool NotifyOnTaskReminder { get; set; }
        public bool NotifyOnTaskCompleted { get; set; }
        public int TimeZoneOffsetMinutes { get; set; }

        public ApplicationUser User { get; set; }

        public NotificationSettingsModel()
        {
            NotifyOnTaskAdded = true;
            NotifyOnTaskChanged = true;
            NotifyOnTaskDeleted = true;
            NotifyOnTaskReminder = true;
            NotifyOnTaskCompleted = true;
        }
    }
}