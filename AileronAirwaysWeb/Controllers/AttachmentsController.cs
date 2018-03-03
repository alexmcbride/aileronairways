using Echelon.TimelineApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IOFile = System.IO.File;

namespace AileronAirwaysWeb.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly ITimelineService _api;
        private readonly IHostingEnvironment _env;

        public AttachmentsController(ITimelineService api, IHostingEnvironment env)
        {
            _api = api;
            _env = env;
        }

        private string GetCacheFolder()
        {
            return Path.Combine(_env.WebRootPath, "cache");
        }

        // GET: Attachments
        [HttpGet("Events/{eventId}/Attachments")]
        public async Task<ActionResult> Index(string eventId)
        {
            var attachments = await Attachment.GetAttachmentsAsync(_api, eventId);

            ViewBag.EventId = eventId;

            return View(attachments);
        }

        // GET: Attachments/Details/5
        [HttpGet("Attachments/{attachmentId}/Download")]
        public async Task<ActionResult> Download(string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);

            await attachment.DownloadAsync(_api, GetCacheFolder());

            // TODO: write this to deliver file.
            return Redirect(attachment.FileName);
        }

        // POST: Attachments/Create
        [HttpPost("Events/{eventId}/Attachments/Upload")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(string eventId, List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string temp = Path.GetTempFileName();

                    try
                    {
                        // Copy file to temp directory.
                        using (var stream = IOFile.OpenWrite(temp))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create attachment and upload to AWS
                        var attachment = await Attachment.CreateAsync(_api, eventId, file.FileName);
                        await attachment.UploadAsync(_api, temp);
                    }
                    finally
                    {
                        // Cleanup temp file.
                        if (IOFile.Exists(temp))
                        {
                            IOFile.Delete(temp);
                        }
                    }
                }
            }

            //_flash.Message($"Uploaded {files.Count} attachments");

            return RedirectToAction(nameof(Index), new { eventId });
        }

        // GET: Attachments/Delete/5
        [HttpGet("Events/{eventId}/Attachments/{attachmentId}/Delete")]
        public async Task<ActionResult> Delete(string eventId, string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);

            ViewBag.EventId = eventId;

            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost("Events/{eventId}/Attachments/{attachmentId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string eventId, string attachmentId, IFormCollection collection)
        {
            // Delete from API.
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);
            await attachment.DeleteAsync(_api, GetCacheFolder());

            //_flash.Message($"Deleted attachment");

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}