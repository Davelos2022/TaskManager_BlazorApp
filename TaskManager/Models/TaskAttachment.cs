using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskAttachment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required, StringLength(100)]
        public string ContentType { get; set; } = null!;

        [Required]
        public byte[] Content { get; set; } = null!;

        [ForeignKey(nameof(TaskId))]
        public TaskModel? Task { get; set; }
    }
}
