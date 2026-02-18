using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskModel>> GetTasksFromSQL(ApplicationUser? user = null);
        Task AddTaskToUser(ApplicationUser user, TaskModel task);
        Task RemoveTask(TaskModel task);
        Task UpdateTask(ApplicationUser user,TaskModel task);
        Task UpdateTasksSorting(List<TaskModel> tasks, List<string> sortOrder);
    }
}