using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using TaskManager.Data;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class UtilService : IUtilService
    {
        #region Properties
        private readonly IJSRuntime _jsRuntime;

        #endregion

        #region Initialiation
        public UtilService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(_jsRuntime));
        }
        #endregion

        #region Methods
        public bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public bool IsStrongPassword(string password)
        {
            bool Check(bool condition, Func<string, bool> predicate) => !condition || predicate(password);

            return !string.IsNullOrEmpty(password) &&
                   password.Length >= ApplicationConstants.MIN_LEGHT_PASSWORD &&
                   Check(ApplicationConstants.REQUIRQ_UPPERCASE, p => p.Any(char.IsUpper)) &&
                   Check(ApplicationConstants.REQUIRQ_LOWERCASE, p => p.Any(char.IsLower)) &&
                   Check(ApplicationConstants.REQUIRQ_DIGIT, p => p.Any(char.IsDigit)) &&
                   Check(ApplicationConstants.REQUIRE_SPECIAL_CHARACTER, p => p.Any(c => !char.IsLetterOrDigit(c)));
        }

        public string FormatDate(DateTime date, string format = "dd.MM.yyyy") =>
        date.ToString(format);

        #endregion
    }
}