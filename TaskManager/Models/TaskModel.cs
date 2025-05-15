using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Attributes;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class TaskModel
    {
        #region Properties
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = ApplicationConstants.ERROR_EMTY)]
        [StringLength(ApplicationConstants.MAX_LEGHT_NAME_TASK, ErrorMessage = ApplicationConstants.ERROR_VALUE_NAME_TASK)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public PriorityModel Priority { get; set; } = PriorityModel.Low;
      
        [Required(ErrorMessage = ApplicationConstants.ERROR_VALUE)]
        [DateValidation]
        public DateTime? DueDate { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public DateTime? LastReminderSent { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")] public ApplicationUser? User { get; set; }
        #endregion

        public TaskModel Clone()
        {
            return new TaskModel
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                Priority = this.Priority,
                DueDate = this.DueDate,
                IsCompleted = this.IsCompleted,
            };
        }
    }
}