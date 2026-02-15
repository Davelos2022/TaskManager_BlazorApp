using System.ComponentModel.DataAnnotations;
using TaskManager.Data;

namespace TaskManager.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DateValidationAttribute : ValidationAttribute
    {
        public DateValidationAttribute()
        {
            ErrorMessage = ApplicationConstants.ERROR_VALUE_DATE;
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime date) return false;
            return date >= DateTime.UtcNow;
        }
    }
}
