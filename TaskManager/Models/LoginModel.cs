using System.ComponentModel.DataAnnotations;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [EmailAddress(ErrorMessage = ApplicationConstants.ERROR_VALUE_EMAIL)]
        public string? Email { get; set; }

        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        public string? Password { get; set; }
    }
}