using System.ComponentModel.DataAnnotations;
using TaskManager.Data;

namespace TaskManager.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ContainsSpecialCharacterAttribute : ValidationAttribute
    {
        public ContainsSpecialCharacterAttribute()
        {
            ErrorMessage = ApplicationConstants.ERROR_VALUE_PASSWORD_CHAR;
        }

        public override bool IsValid(object? value)
        {
            string? input = value as string;
            if (string.IsNullOrEmpty(input))
                return true;

            return input.Any(c => !char.IsLetterOrDigit(c));
        }
    }
}
