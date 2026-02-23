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
        public TaskRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;
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

        public async Task AddAttachmentAsync(TaskAttachment attachment)
        {
            _dbContext.TaskAttachments.Add(attachment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<AttachmentInfo>> GetAttachmentsInfoAsync(Guid taskId)
        {
            return await _dbContext.TaskAttachments
                .Where(a => a.TaskId == taskId)
                .Select(a => new AttachmentInfo
                {
                    Id = a.Id,
                    FileName = a.FileName
                }).ToListAsync();
        }

        public async Task<List<TaskAttachment>> GetAttachmentsAsync(Guid taskId)
        {
            return await _dbContext.TaskAttachments
                      .Where(a => a.TaskId == taskId)
                      .ToListAsync();
        }

        public async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            var att = await _dbContext.TaskAttachments.FindAsync(attachmentId);
            if (att != null)
            {
                _dbContext.TaskAttachments.Remove(att);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UploadAsync(Guid taskId, string fileName, string contentType, byte[] content)
        {
            var att = new TaskAttachment
            {
                TaskId = taskId,
                FileName = fileName,
                ContentType = contentType,
                Content = content
            };

            await AddAttachmentAsync(att);
        }

        #endregion
    }
}