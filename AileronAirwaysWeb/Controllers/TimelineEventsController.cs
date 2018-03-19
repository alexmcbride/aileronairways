using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelineEventsController : Controller
    {
        private readonly TimelineRepository _repo;
        private readonly IFlashService _flash;

        public TimelineEventsController(TimelineRepository repo, IFlashService flash)
        {
            _repo = repo;
            _flash = flash;
        }

        [HttpGet("Timelines/{timelineId}/Events")]
        public ActionResult Index(string timelineId)
        {
            var timeline = _repo.GetTimelineWithEvents(timelineId);
            var events = timeline.TimelineEvents
                .OrderBy(e => e.EventDateTime)
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
            Timeline timeline = _repo.GetTimelineWithEvents(timelineId);
            TimelineEvent timelineEvent = _repo.GetTimelineEventWithAttachments(eventId);

            ViewBag.TimelineId = timeline.Id;
            ViewBag.TimelineTitle = timeline.Title;
            ViewBag.EventId = eventId;

            // Get next and previous events
            ViewBag.NextEvent = await _repo.GetNextEventAsync(timelineEvent);
            ViewBag.PreviousEvent = await _repo.GetPreviousEventAsync(timelineEvent);

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
                TimelineEvent evt = await _repo.CreateTimelineEventAsync(
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
        public ActionResult Edit(string timelineId, string eventId)
        {
            TimelineEvent timelineEvent = _repo.GetTimelineEventWithAttachments(eventId);

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
                TimelineEvent evt = _repo.GetTimelineEventWithAttachments(eventId);
                evt.Title = vm.Title;
                evt.Description = vm.Description;
                evt.EventDateTime = vm.EventDateTime;
                evt.Location = vm.Location;
                await _repo.EditTimelineEventAsync(evt);

                _flash.Message($"Event '{evt.Title}' edited!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/Delete/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Delete")]
        public ActionResult Delete(string timelineId, string eventId)
        {
            TimelineEvent evt = _repo.GetTimelineEventWithAttachments(eventId);

            return PartialView(new TimelineEventViewModel
            {
                Id = evt.Id,
                TimelineId = timelineId,
                Description = evt.Description,
                EventDateTime = evt.EventDateTime,
                Location = evt.Location,
                Title = evt.Title
            });
        }

        // POST: Timelines/Delete/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string timelineId, string eventId, IFormCollection collection)
        {
            var evt = _repo.GetTimelineEventWithAttachments(eventId);
            await _repo.DeleteTimelineEventAsync(evt);

            _flash.Message("Deleted timeline event");

            return Ok("OK " + eventId);
        }
    }
}