using System.ComponentModel.DataAnnotations;
using TaskManager.Attributes;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class RegisterModel 
    {
        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [MinLength(ApplicationConstants.MIN_LEGHT_NAME, ErrorMessage = ApplicationConstants.ERROR_VALUE_NAME_LEGHT_MIN)]
        [StringLength(ApplicationConstants.MAX_LEGHT_NAME, ErrorMessage = ApplicationConstants.ERROR_VALUE_NAME_LEGHT_MAX)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [EmailAddress(ErrorMessage = ApplicationConstants.ERROR_VALUE_EMAIL)]
        public string? Email { get; set; }

        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [TelegramIDValidation]
        public string? TelegramID { get; set; }

        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [MinLength(ApplicationConstants.MIN_LEGHT_PASSWORD, ErrorMessage = ApplicationConstants.ERROR_VALUE_PASSWORD_LEGTH)]
        [ContainsSpecialCharacter]
        [ContainsDigit]
        [ContainsUppercase]
        public string? Password { get; set; }

        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [Compare("Password", ErrorMessage = ApplicationConstants.ERROR_PASSWORD_COMPARE)]
        public string? ConfirmPassword { get; set; }

        public string? ChatID { get; set; }
    }
}