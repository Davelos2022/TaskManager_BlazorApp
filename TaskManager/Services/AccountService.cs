using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class AccountService : IAccountService
    {
        #region Properties
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuthenticationStateProvider _authenticationState;
        #endregion

        #region Initialiation
        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IServiceScopeFactory scopeFactory, ApplicationDbContext context, AuthenticationStateProvider stateProvider)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authenticationState = stateProvider ?? throw new ArgumentNullException(nameof(stateProvider));
        }
        #endregion

        #region Controller Account
        public async Task<IdentityResult> RegisterUser(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                TelegramID = model.TelegramID,
                FirstName = model.FirstName,
                ChatId = model.ChatID
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        public async Task<SignInResult> LoginUser(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            return result;
        }

        public async Task<IdentityResult> LogoutUser()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex}");
                return IdentityResult.Failed();
            }
        }

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            try
            {
                var authState = await _authenticationState.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        return await userManager.GetUserAsync(user);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return null;
            }
        }

        public async Task<bool> CheckAdminStatus()
        {
            var user = await GetCurrentUser();
            if (user != null) return user.AdminUser;
            else return false;
        }

        public async Task<IdentityResult> UpdateUser(ApplicationUser updatedUser)
        {
            var existingUser = await _userManager.FindByIdAsync(updatedUser.Id);

            if (existingUser != null)
            {
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.Email = updatedUser.Email;
                existingUser.TelegramID = updatedUser.TelegramID;
                existingUser.AvatarImage = updatedUser.AvatarImage;

                return await _userManager.UpdateAsync(existingUser);
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> ChangePassword(ApplicationUser user, string oldPassword, string newPassword)
        {
            var currentUser = await _userManager.FindByIdAsync(user.Id);
            bool isCurrentValid = await _userManager.CheckPasswordAsync(currentUser, oldPassword);

            if (isCurrentValid)
            {
                var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);
                return result;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = ApplicationConstants.ERROR_PASSWORD_CURRENT
                });
            }
        }

        public async Task<ApplicationUser?> GetUserByEmail(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<ApplicationUser?> GetUserByID(string ID)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == ID);

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ApplicationUser[]> GetAllUsers()
        {
            try
            {
                ApplicationUser? currentUser = await GetCurrentUser();

                if (currentUser != null)
                {
                    var allUsers = _context.Users
                    .Where(u => u.Id != currentUser.Id)
                     .ToArray();

                    return allUsers;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> SignInFromTelegramApp(string rawInitData, HttpContext http)
        {
            Dictionary<string, string> dict = QueryHelpers.ParseQuery(rawInitData)
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            string chatId = "";

            if (dict.TryGetValue("user", out var jsonUser))
            {
                try
                {
                    using var doc = JsonDocument.Parse(jsonUser);
                    chatId = doc.RootElement.GetProperty("id").GetInt32().ToString() ?? "";
                }
                catch
                {
                    return false;
                }
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.ChatId == chatId);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return true;
            }

            return false;
        }

        public async Task<ApplicationUser?> GetUserByIDTelegram(string tgID)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.TelegramID == tgID);

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion
    }
}