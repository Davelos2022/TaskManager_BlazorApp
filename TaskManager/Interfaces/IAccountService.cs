using Microsoft.AspNetCore.Identity;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUser(RegisterModel model);
        Task<SignInResult> LoginUser(LoginModel model);
        Task<IdentityResult> LogoutUser();
        Task<bool> CheckAdminStatus();
        Task<ApplicationUser?> GetCurrentUser();
        Task<ApplicationUser[]> GetAllUsers();
        Task<IdentityResult> UpdateUser(ApplicationUser user);
        Task<ApplicationUser?> GetUserByEmail(string email);
        Task<ApplicationUser?> GetUserByID(string ID);
        Task<IdentityResult> ChangePassword(ApplicationUser user, string oldPassword, string newPassword);
        Task<bool> SignInFromTelegramApp(string rawInitData, HttpContext http);
        Task<ApplicationUser?> GetUserByIDTelegram(string tgID);
    }
}