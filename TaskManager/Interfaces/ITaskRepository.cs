using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetDueTasksAsync(DateTime now, TimeSpan reminderThreshold);
    }
}