using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelineEventsController : Controller
    {
        private readonly ITimelineService _api;
        private readonly IFlashService _flash;

        public TimelineEventsController(ITimelineService api, IFlashService flash)
        {
            _api = api;
            _flash = flash;
        }

        [HttpGet("Timelines/{timelineId}/Events")]
        public async Task<ActionResult> Index(string timelineId)
        {
            var timeline = await Timeline.GetTimelineAsync(_api, timelineId);
            var linkedEvents = await TimelineEvent.GetEventsAsync(_api, timelineId);

            // Download all TimelineEvent objects at the same time.
            var tasks = linkedEvents.Select(e => TimelineEvent.GetEventAsync(_api, e.TimelineEventId));
            var timelineEvents = (await Task.WhenAll(tasks))
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.EventDateTime)
                .ToList();

            // Extra data needed by view.
            ViewBag.TimelineId = timelineId;
            ViewBag.TimelineTitle = timeline.Title;

            return View(timelineEvents);
        }

        [HttpGet("Timelines/{timelineId}/Events/{eventId}")]
        public async Task<ActionResult> Details(string timelineId, string eventId)
        {
            Timeline timeline = await Timeline.GetTimelineAsync(_api, timelineId);
            TimelineEvent timelineEvent = await TimelineEvent.GetEventAsync(_api, eventId);

            ViewBag.TimelineId = timeline.Id;
            ViewBag.TimelineTitle = timeline.Title;

            return View(timelineEvent);
        }

        [HttpGet("Timelines/{timelineId}/Events/Create")]
        public ActionResult Create(string timelineId)
        {
            ViewBag.TimelineId = timelineId;

            // Create new blank timeline and set default values
            TimelineEvent timelineEvent = new TimelineEvent();
            timelineEvent.EventDateTime = DateTime.Now;

            return View(timelineEvent);
        }

        [HttpPost("Timelines/{timelineId}/Events/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string timelineId, IFormCollection collection)
        {
            DateTime date = DateTime.Parse(Request.Form["EventDateTime"]);

            TimelineEvent evt = await TimelineEvent.CreateAsync(_api,
                Request.Form["Title"],
                Request.Form["Description"],
                date,
                Request.Form["Location"]);

            await evt.LinkEventAsync(_api, timelineId);

            _flash.Message($"Event '{evt.Title}' added!");

            ViewBag.TimelineId = timelineId;

            return RedirectToAction(nameof(Index), new { timelineId });
        }

        //GET: Timelines/Edit/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Edit")]
        public async Task<ActionResult> Edit(string timelineId, string eventId)
        {
            TimelineEvent timelineEvent = await TimelineEvent.GetEventAsync(_api, eventId);

            ViewBag.TimelineId = timelineId;

            return View(timelineEvent);
        }

        //POST: Timelines/Edit/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string timelineId, string eventId, IFormCollection collection)
        {
            var date = DateTime.Parse(Request.Form["EventDateTime"]);

            var evt = await TimelineEvent.GetEventAsync(_api, eventId);
            evt.Title = Request.Form["Title"];
            evt.Description = Request.Form["Description"];
            evt.EventDateTime = date;
            evt.Location = Request.Form["Location"];

            await evt.EditAsync(_api);

            _flash.Message($"Event '{evt.Title}' edited!");

            return RedirectToAction("Details", "TimelineEvents", new { timelineId, eventId = evt.Id });
        }

        // GET: Timelines/Delete/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Delete")]
        public async Task<ActionResult> Delete(string timelineId, string eventId)
        {
            TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, eventId);

            // Make sure to unlink from timeline first.
            // TODO: IdeaGen API currently broken.
            //await evt.UnlinkEventAsync(_api, timelineId);

            await evt.DeleteAsync(_api);

            _flash.Message($"Event '{evt.Title}' deleted!");

            return RedirectToAction("Index", new { id = timelineId });
        }

        //// POST: Timelines/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}