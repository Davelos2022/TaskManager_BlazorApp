using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Bots.Telegram
{
    public class TelegramRegistrState
    {
        public long ChatId { get; set; }
        public RegistrationStepModel CurrentStep { get; set; } = RegistrationStepModel.Initial;
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Password { get; set; }
        public string? TelegramUsername { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsExpired => DateTime.UtcNow.Subtract(CreatedAt).TotalMinutes > ApplicationConstants.REGISTRATION_EXPIRY_MINUTES;

        public bool IsComplete =>
            !string.IsNullOrEmpty(Email) &&
            !string.IsNullOrEmpty(FirstName) &&
            !string.IsNullOrEmpty(Password);
    }
}