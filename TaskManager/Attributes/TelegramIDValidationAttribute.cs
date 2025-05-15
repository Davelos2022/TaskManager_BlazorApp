using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TaskManager.Data;

namespace TaskManager.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TelegramIDValidationAttribute : ValidationAttribute
    {
        public TelegramIDValidationAttribute()
        {
            ErrorMessage = ApplicationConstants.ERROR_VALUE_TG;
        }

        public override bool IsValid(object? value)
        {
            var tgID = value as string;
            if (string.IsNullOrWhiteSpace(tgID))
            {
                return false;
            }

            var regex = new Regex(@"^@[A-Za-z0-9_]{5,32}$");
            return regex.IsMatch(tgID);
        }
    }
}
