using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class NotificationSettingsService : INotificationSettingsService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Initialiation
        public NotificationSettingsService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
        }
        #endregion

        #region Methods
        public async Task<NotificationSettingsModel> GetSettingsAsync(ApplicationUser user)
        {
            try
            {  
                var settings = await _context.NotificationSettings
                    .FirstOrDefaultAsync(ns => ns.UserId == user.Id);

                if (settings == null)
                {
                    settings = new NotificationSettingsModel { UserId = user.Id };
                    _context.NotificationSettings.Add(settings);
                    await _context.SaveChangesAsync();
                }

                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return null;
            }
        }

        public async Task SaveSettingsAsync(NotificationSettingsModel settings)
        {
            try
            {
                _context.NotificationSettings.Update(settings);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }
        #endregion
    }
}