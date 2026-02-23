using Microsoft.AspNetCore.Components.Forms;
using TaskManager.Models;

namespace TaskManager.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetDueTasksAsync(DateTime now, TimeSpan reminderThreshold);
        Task UploadAsync(Guid taskId, string fileName, string contentType, byte[] content);
        Task AddAttachmentAsync(TaskAttachment attachment);
        Task<IEnumerable<AttachmentInfo>> GetAttachmentsInfoAsync(Guid taskId);
        Task<List<TaskAttachment>> GetAttachmentsAsync(Guid taskId);
        Task DeleteAttachmentAsync(Guid attachmentId);
    }
}