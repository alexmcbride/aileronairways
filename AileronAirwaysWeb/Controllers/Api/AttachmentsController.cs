using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/attachments")]
    public class AttachmentsController : Controller
    {
        private readonly TimelineRepository _repo;

        public AttachmentsController(TimelineRepository repo)
        {
            _repo = repo;
        }

        // GET: api/attachments/all/abc123
        [HttpGet("all/{eventId}")]
        public IActionResult GetAll(string eventId)
        {
            var attachments = _repo.GetAttachments(eventId).Select(a => new AttachmentViewModel
            {
                Id = a.Id,
                Title = a.Title,
                TimelineEventId = a.TimelineEventId
            });

            return Ok(attachments);
        }

        // GET: api/attachments/abc123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var attachment = _repo.GetAttachment(id);

            return Ok(new AttachmentViewModel
            {
                Id = attachment.Id,
                Title = attachment.Title,
                TimelineEventId = attachment.TimelineEventId
            });
        }

        // DELETE: api/attachments/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repo.DeleteAttachmentAsync(id);

            return Ok();
        }
    }
}
