using System.ComponentModel.DataAnnotations;
using TaskManager.Attributes;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class ProfileModel : IValidatableObject
    {
        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [MinLength(ApplicationConstants.MIN_LEGHT_NAME, ErrorMessage = ApplicationConstants.ERROR_VALUE_NAME_LEGHT_MIN)]
        [StringLength(ApplicationConstants.MAX_LEGHT_NAME, ErrorMessage = ApplicationConstants.ERROR_VALUE_NAME_LEGHT_MAX)]
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? TelegramID { get; set; }
        public string? ChatID { get; set; }
        public string? PreviewImageUrl { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;

        [ContainsSpecialCharacter]
        [ContainsDigit]
        [ContainsUppercase]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = ApplicationConstants.ERROR_PASSWORD_COMPARE)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool ChangePasswordRequested { get; set; } = false;

        public ProfileModel(ApplicationUser user)
        {
            FirstName = user.FirstName;
            Email = user.Email;
            TelegramID = user.TelegramID;
            PreviewImageUrl = user.AvatarImage != null ?
                 $"data:image/png;base64,{Convert.ToBase64String(user.AvatarImage)}" : null;
        }

        #region Validation Context
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ChangePasswordRequested)
            {
                if (string.IsNullOrWhiteSpace(OldPassword))
                {
                    yield return new ValidationResult(ApplicationConstants.ERROR_EMTY, new[] { nameof(OldPassword) });
                }

                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    yield return new ValidationResult(ApplicationConstants.ERROR_EMTY, new[] { nameof(NewPassword) });
                }
                else if (NewPassword.Length < ApplicationConstants.MIN_LEGHT_PASSWORD)
                {
                    yield return new ValidationResult(
                        $"{ApplicationConstants.ERROR_VALUE_PASSWORD_LEGTH}",
                        new[] { nameof(NewPassword) }
                    );
                }

                if (string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    yield return new ValidationResult(ApplicationConstants.ERROR_PASSWORD_CONFIM, new[] { nameof(ConfirmPassword) });
                }
            }
        }

        #endregion
    }
}