namespace TaskManager.Models
{
    public class FileUploadModel
    {
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public Guid Id { get; set; }
        public bool IsNewFile { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
