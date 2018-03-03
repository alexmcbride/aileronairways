using Echelon.TimelineApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IOFile = System.IO.File;

namespace AileronAirwaysWeb.Controllers
{
    [Route("attachments")]
    public class AttachmentsController : Controller
    {
        private readonly ITimelineService _api;
        private readonly IHostingEnvironment _env;

        public AttachmentsController(ITimelineService api, IHostingEnvironment env)
        {
            _api = api;
            _env = env;
        }

        // GET: Attachments
        [HttpGet("{eventId}")]
        public async Task<ActionResult> Index(string eventId)
        {
            var attachments = await Attachment.GetAttachmentsAsync(_api, eventId);

            ViewBag.EventId = eventId;

            return View(attachments);
        }

        [HttpGet("download/{attachmentId}")]
        public async Task<ActionResult> Download(string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);

            await attachment.DownloadOrCacheAsync(_api);

            return File(attachment.FileName, attachment.ContentType, attachment.Title);
        }

        // POST: Attachments/Create
        [HttpPost("upload/{eventId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(string eventId, List<IFormFile> files)
        {
            files = files.Where(f => f.Length > 0).ToList();
            foreach (var file in files)
            {
                // Create attachment and upload to AWS
                await Attachment.CreateAndUploadAsync(_api, eventId, file.FileName, file.OpenReadStream());
            }

            //_flash.Message($"Uploaded {files.Count} attachments");

            return RedirectToAction(nameof(Index), new { eventId });
        }

        // GET: Attachments/Delete/5
        [HttpGet("{eventId}/delete/{attachmentId}")]
        public async Task<ActionResult> Delete(string eventId, string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);

            ViewBag.EventId = eventId;

            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost("{eventId}/delete/{attachmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string eventId, string attachmentId, IFormCollection collection)
        {
            // Delete from API.
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);
            await attachment.DeleteAsync(_api);

            //_flash.Message($"Deleted attachment");

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}