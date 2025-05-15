using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class TaskService : ITaskService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        #endregion

        #region Initialiation
        public TaskService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
        }
        #endregion

        #region Controller Tasks
        private async Task<string?> GetUserId()
        {
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity.IsAuthenticated)
                {
                    return user.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} ");
                return null;
            }
        }

        public async Task<List<TaskModel>> GetTasksFromSQL(ApplicationUser? user = null)
        {
            var userId = user == null ? await GetUserId() : user.Id;

            if (userId == null) return new List<TaskModel>();

            return await _context.Tasks
             .Where(t => t.UserId == userId)
             .OrderBy(t => t.SortOrder)
             .ToListAsync();
        }

        public async Task AddTaskToUser(ApplicationUser user, TaskModel task)
        {
            try
            {
                var maxOrder = await _context.Tasks.MaxAsync(t => (int?)t.SortOrder) ?? -1;

                task.UserId = user.Id;
                task.SortOrder = maxOrder + 1;

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task RemoveTask(TaskModel task)
        {
            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} ");
                throw;
            }
        }

        public async Task UpdateTask(TaskModel task)
        {
            try
            {
                var existingTask = await _context.Tasks.FindAsync(task.Id);

                if (existingTask != null)
                {
                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.Priority = task.Priority;
                    existingTask.DueDate = task.DueDate;
                    existingTask.IsCompleted = task.IsCompleted;

                    _context.Tasks.Update(existingTask);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"Task with ID {task.Id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task UpdateTasksSorting(List<TaskModel> tasks, List<string> newOrders)
        {
            var taskIds = newOrders.Select(id => Guid.Parse(id)).ToList();

            for (int i = 0; i < taskIds.Count; i++)
            {
                var task = tasks.First(t => t.Id == taskIds[i]);
                task.SortOrder = i;
            }

            await _context.SaveChangesAsync();
        }
        #endregion
    }
}