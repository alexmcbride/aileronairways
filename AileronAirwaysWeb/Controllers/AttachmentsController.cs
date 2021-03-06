﻿using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    /// <summary>
    /// Controller for uploading and downloading attachments
    /// </summary>
    [Route("attachments")]
    public class AttachmentsController : Controller
    {
        private readonly TimelineRepository _repo;
        private readonly IFlashService _flash;

        public AttachmentsController(TimelineRepository repo, IFlashService flash)
        {
            _repo = repo;
            _flash = flash;
        }

        // GET: Attachments/Download/<ID>
        [HttpGet("download/{attachmentId}")]
        public async Task<ActionResult> Download(string attachmentId)
        {
            var attachment = await _repo.DownloadAttachmentAsync(attachmentId);

            return File(attachment.FileName, attachment.ContentType, attachment.Title);
        }

        // POST: Attachments/Create
        [HttpPost("upload/{eventId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(string eventId, List<IFormFile> files, string tab)
        {
            if (files.Any())
            {
                foreach (var file in files)
                {
                    // Create attachment and upload to AWS
                    await _repo.CreateAttachmentAsync(eventId, file.FileName, file.OpenReadStream());
                }

                var s = files.Count > 1 ? "s" : "";
                _flash.Message($"Uploaded {files.Count} attachment{s}");
            }
            else
            {
                _flash.Message("No attachments uploaded", FlashType.Info);
            }

            var @event = _repo.GetTimelineEvent(eventId);
            return RedirectToAction("Details", "TimelineEvents", new { eventId, timelineId = @event.TimelineId, tab });
        }

        // GET: Attachments/<ID>/Delete/<ID>
        [HttpGet("{eventId}/delete/{attachmentId}")]
        public ActionResult Delete(string eventId, string attachmentId)
        {
            var attachment = _repo.GetAttachment(attachmentId);

            ViewBag.EventId = eventId;

            return PartialView(attachment);
        }

        // POST: Attachments/<ID>/Delete/<ID>
        [HttpPost("{eventId}/delete/{attachmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string eventId, string attachmentId, IFormCollection collection)
        {
            // Delete from API.
            await _repo.DeleteAttachmentAsync(attachmentId);

            _flash.Message($"Deleted attachment");

            var @event = _repo.GetTimelineEvent(eventId);
            return Ok("OK " + attachmentId);
        }
    }
}