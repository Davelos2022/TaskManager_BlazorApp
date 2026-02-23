namespace TaskManager.Data
{
    public class NotificationOptions
    {
        public int ReminderLeadHours { get; set; }
        public int ResendIntervalMinutes { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}
