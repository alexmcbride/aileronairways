using Echelon.TimelineApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/attachments")]
    public class AttachmentsController : Controller
    {
        private readonly ITimelineService _api;

        public AttachmentsController(ITimelineService api)
        {
            _api = api;
        }

        // GET: api/attachments/all/abc123
        [HttpGet("all/{eventId}")]
        public async Task<IActionResult> GetAll(string eventId)
        {
            var attachments = await Attachment.GetAttachmentsAsync(_api, eventId);

            return Ok(attachments);
        }

        // GET: api/attachments/abc123
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, id);

            return Ok(attachment);
        }

        // DELETE: api/attachments/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, id);

            await attachment.DeleteAsync(_api);

            return Ok();
        }
    }
}
