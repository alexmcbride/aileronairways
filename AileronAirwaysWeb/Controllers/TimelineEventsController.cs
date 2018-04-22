using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    /// <summary>
    /// Controller for adding, editing, deleting timeline events.
    /// </summary>
    public class TimelineEventsController : Controller
    {
        private readonly TimelineRepository _repo;
        private readonly IFlashService _flash;

        public TimelineEventsController(TimelineRepository repo, IFlashService flash)
        {
            _repo = repo;
            _flash = flash;
        }

        // GET: Timelines/<ID>/Events
        [HttpGet("Timelines/{timelineId}/Events")]
        public ActionResult Index(string timelineId)
        {
            var events = LoadTimelineEventsFromRepo(timelineId);

            return View(events);
        }

        // GET: Timelines/<ID>/Events/<ID>
        [HttpGet("Timelines/{timelineId}/Events/{eventId}")]
        public async Task<ActionResult> Details(string timelineId, string eventId, string tab)
        {
            Timeline timeline = _repo.GetTimelineWithEvents(timelineId);
            TimelineEvent timelineEvent = _repo.GetTimelineEventWithAttachments(eventId);

            ViewBag.TimelineId = timeline.Id;
            ViewBag.TimelineTitle = timeline.Title;
            ViewBag.EventId = eventId;
            ViewBag.Tab = string.IsNullOrEmpty(tab) ? "overview" : tab;

            // Get next and previous events
            ViewBag.NextEvent = await _repo.GetNextEventAsync(timelineEvent);
            ViewBag.PreviousEvent = await _repo.GetPreviousEventAsync(timelineEvent);
            if (timelineEvent.Location == null)
            {
                timelineEvent.Location = "0,0";
            }

            return View(timelineEvent);
        }

        // GET: Timelines/<ID>/Events/Create
        [HttpGet("Timelines/{timelineId}/Events/Create")]
        public ActionResult Create(string timelineId)
        {
            var vm = new TimelineEventViewModel
            {
                EventDateTime = DateTime.Now
            };

            return PartialView(vm);
        }

        // GET: Timelines/<ID>/Events/Create
        [HttpPost("Timelines/{timelineId}/Events/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string timelineId, [Bind("Title,Description,EventDateTime")] TimelineEventViewModel vm)
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

        // GET: Timelines/<ID>/Events/<ID>/Edit
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Edit")]
        public ActionResult Edit(string timelineId, string eventId)
        {
            TimelineEvent timelineEvent = _repo.GetTimelineEvent(eventId);

            var vm = new TimelineEventViewModel
            {
                Title = timelineEvent.Title,
                Description = timelineEvent.Description,
                EventDateTime = timelineEvent.EventDateTime,
                Location = timelineEvent.Location
            };

            return PartialView(vm);
        }

        //POST: Timelines/<ID>/Events/<ID>/Edit
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string timelineId, string eventId, [Bind("Title,Description,EventDateTime")]  TimelineEventViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent evt = _repo.GetTimelineEvent(eventId);
                evt.Title = vm.Title;
                evt.EventDateTime = vm.EventDateTime;
                evt.Description = vm.Description;
                await _repo.EditTimelineEventAsync(evt);

                _flash.Message($"Event '{evt.Title}' edited!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/<ID>/Events/<ID>/Delete
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Delete")]
        public ActionResult Delete(string timelineId, string eventId)
        {
            TimelineEvent evt = _repo.GetTimelineEvent(eventId);

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

        // POST: Timelines/<ID>/Events/<ID>/Delete
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string timelineId, string eventId, IFormCollection collection)
        {
            var evt = _repo.GetTimelineEvent(eventId);
            await _repo.DeleteTimelineEventAsync(evt);

            _flash.Message($"Deleted '{evt.Title}' event");

            return Ok("OK " + eventId);
        }

        // Partial GET: /TimelineEvents/Description/<ID>
        [HttpGet]
        public ActionResult Description(string id)
        {
            TimelineEvent @event = _repo.GetTimelineEvent(id);

            return PartialView("Description", @event);
        }

        // Partial GET: /TimelineEvents/DescriptionEdit/<ID>
        [HttpGet]
        public ActionResult DescriptionEdit(string id)
        {
            TimelineEvent @event = _repo.GetTimelineEvent(id);

            return PartialView("DescriptionEdit", new EditDescriptionViewModel
            {
                Id = @event.Id,
                Description = @event.Description
            });
        }

        // Partial POST: /TimelineEvents/DescriptionEditPost/<ID>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DescriptionEditPost(string id, [Bind("Id,Description")] EditDescriptionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent @event = _repo.GetTimelineEvent(id);
                @event.Description = vm.Description;
                await _repo.EditDescriptionAsync(@event);
                return PartialView("Description", @event);
            }

            return PartialView("DescriptionEdit", vm);
        }

        // Partial POST: Timelines/<ID>/Events/<ID>/EditEventLocation
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/EditEventLocation")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEventLocation(string eventId, [Bind("Location")]  TimelineEventLocationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent evt = _repo.GetTimelineEvent(eventId);
                evt.Location = vm.Location;
                await _repo.EditEventLocationAsync(evt);

                _flash.Message($"Event location saved!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        // Partial GET: TimelineEvents/TimelineZoomedInPartial/<ID>
        [HttpGet]
        public ActionResult TimelineZoomedInPartial(string id)
        {
            var events = LoadTimelineEventsFromRepo(id);

            return PartialView("_TimelineZoomedIn", events);
        }

        // Partial GET: TimelineEvents/TimelineZoomedOutPartial/<ID>
        [HttpGet]
        public ActionResult TimelineZoomedOutPartial(string id)
        {
            var events = LoadTimelineEventsFromRepo(id);

            return PartialView("_TimelineZoomedOut", events);
        }

        /// <summary>
        /// Prepares the timeline event stuff used by zoomed in and out partials
        /// </summary>
        private IEnumerable<TimelineEvent> LoadTimelineEventsFromRepo(string id)
        {
            var timeline = _repo.GetTimelineWithEvents(id);
            var events = timeline.TimelineEvents.OrderBy(e => e.EventDateTime);

            ViewBag.TimelineId = id;
            ViewBag.TimelineTitle = timeline.Title;
            ViewBag.TimelineCreationTimeStamp = timeline.CreationTimeStamp;

            return events;
        }

        // Partial GET: TimelineEvents/FlashMessages
        [HttpGet]
        public ActionResult FlashMessages()
        {
            if (_flash.HasMessages)
            {
                return PartialView("_FlashMessages");
            }
            return Ok();
        }

        // Partial GET: TimelineEvents/EventsSidebar/<ID>
        [HttpGet]
        public ActionResult EventsSidebar(string id)
        {
            var events = _repo.TimelineEvents
                .Where(e => e.TimelineId == id)
                .OrderBy(e => e.EventDateTime);

            return PartialView("_EventsSidebar", events);
        }
    }
}