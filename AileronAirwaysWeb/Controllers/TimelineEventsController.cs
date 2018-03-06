using AileronAirwaysWeb.Models;
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
            var events = (await Task.WhenAll(tasks))
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.EventDateTime)
                .ToList();

            // Extra data needed by view.
            ViewBag.TimelineId = timelineId;
            ViewBag.TimelineTitle = timeline.Title;
            ViewBag.TimelineCreationTimeStamp = timeline.CreationTimeStamp;

            return View(events);
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
            var vm = new TimelineEventViewModel
            {
                EventDateTime = DateTime.Now
            };

            return PartialView(vm);
        }

        [HttpPost("Timelines/{timelineId}/Events/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string timelineId, [Bind("Title,Description,EventDateTime,Location")] TimelineEventViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent evt = await TimelineEvent.CreateAndLinkAsync(_api,
                    vm.Title,
                    vm.Description,
                    vm.EventDateTime,
                    vm.Location,
                    timelineId);

                _flash.Message($"Event '{evt.Title}' added!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        //GET: Timelines/Edit/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Edit")]
        public async Task<ActionResult> Edit(string timelineId, string eventId)
        {
            TimelineEvent timelineEvent = await TimelineEvent.GetEventAsync(_api, eventId);

            var vm = new TimelineEventViewModel
            {
                Title = timelineEvent.Title,
                Description = timelineEvent.Description,
                EventDateTime = timelineEvent.EventDateTime,
                Location = timelineEvent.Location
            };

            return PartialView(vm);
        }

        //POST: Timelines/Edit/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string timelineId, string eventId, [Bind("Title,Description,EventDateTime,Location")]  TimelineEventViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, eventId);
                evt.Title = vm.Title;
                evt.Description = vm.Description;
                evt.EventDateTime = vm.EventDateTime;
                evt.Location = vm.Location;

                await evt.EditAsync(_api);

                _flash.Message($"Event '{evt.Title}' edited!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/Delete/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Delete")]
        public async Task<ActionResult> Delete(string timelineId, string eventId)
        {
            TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, eventId);

            ViewBag.TimelineId = timelineId;

            return View(evt);
        }

        // POST: Timelines/Delete/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string timelineId, string eventId, IFormCollection collection)
        {
            await TimelineEvent.UnlinkAndDeleteAsync(_api, timelineId, eventId);

            _flash.Message("Deleted timeline event");

            return RedirectToAction(nameof(Index));
        }
    }
}