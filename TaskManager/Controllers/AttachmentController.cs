using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;

namespace TaskManager.Controllers
{
    [Route("attachment")]
    public class AttachmentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AttachmentController(ApplicationDbContext db) => _db = db;

        [HttpGet("download/{id:guid}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var att = await _db.TaskAttachments.FindAsync(id);
            if (att == null) return NotFound();

            Response.Headers.Append("Content-Disposition",
                new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("inline")
                { FileName = att.FileName }.ToString());

            return File(att.Content, att.ContentType);
        }
    }
}
