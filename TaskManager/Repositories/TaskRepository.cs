using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        #region Properties
        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Initialiation
        public TaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods
        public async Task<IEnumerable<TaskModel>> GetDueTasksAsync(DateTime now, TimeSpan reminderThreshold)
        {
            return await _dbContext.Tasks
                .Include(t => t.User)
                .Where(t => !t.IsCompleted &&
                           t.DueDate != null &&
                           (t.DueDate.Value <= now.Add(reminderThreshold) || t.DueDate.Value < now))
                .ToListAsync();
        }

        #endregion
    }
}