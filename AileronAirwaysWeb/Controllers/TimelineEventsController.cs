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
    public class TimelineEventsController : Controller
    {
        private readonly TimelineRepository _repo;
        private readonly IFlashService _flash;


        public TimelineEventsController(TimelineRepository repo, IFlashService flash, ITimelineService api)
        {
            _repo = repo;
            _flash = flash;
        }

        [HttpGet("Timelines/{timelineId}/Events")]
        public ActionResult Index(string timelineId)
        {
            var events = LoadTimelineEventsFromRepo(timelineId);

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
            if (timelineEvent.Location == null)
            {
                timelineEvent.Location = "0,0";
            }

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

        //GET: Timelines/Edit/5
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

        //POST: Timelines/Edit/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string timelineId, string eventId, [Bind("Title,Description,EventDateTime,Location")]  TimelineEventViewModel vm)
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

        // GET: Timelines/Delete/5
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

        // POST: Timelines/Delete/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string timelineId, string eventId, IFormCollection collection)
        {
            var evt = _repo.GetTimelineEvent(eventId);
            await _repo.DeleteTimelineEventAsync(evt);

            _flash.Message($"Deleted '{evt.Title}' event");

            return Ok("OK " + eventId);
        }

        [HttpGet]
        public ActionResult Description(string id)
        {
            TimelineEvent @event = _repo.GetTimelineEvent(id);

            return PartialView("Description", @event);
        }

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

        //POST: Timelines/Edit/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/EditEventLocation")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEventLocation(string eventId, [Bind("Location")]  TimelineEventLocationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                TimelineEvent evt = _repo.GetTimelineEvent(eventId);
                evt.Location = vm.Location;
                await _repo.EditEventLocationAsync(evt);


                _flash.Message($"Event location edited!");

                return Ok("OK " + evt.Id);
            }

            return PartialView(vm);
        }

        [HttpGet]
        public ActionResult TimelineZoomedInPartial(string id)
        {
            var events = LoadTimelineEventsFromRepo(id);

            return PartialView("_TimelineZoomedIn", events);
        }

        [HttpGet]
        public ActionResult TimelineZoomedOutPartial(string id)
        {
            var events = LoadTimelineEventsFromRepo(id);

            return PartialView("_TimelineZoomedOut", events);
        }

        private IEnumerable<TimelineEvent> LoadTimelineEventsFromRepo(string id)
        {
            var timeline = _repo.GetTimelineWithEvents(id);
            var events = timeline.TimelineEvents.OrderBy(e => e.EventDateTime);

            ViewBag.TimelineId = id;
            ViewBag.TimelineTitle = timeline.Title;
            ViewBag.TimelineCreationTimeStamp = timeline.CreationTimeStamp;

            return events;
        }

        [HttpGet]
        public ActionResult FlashMessages()
        {
            return PartialView("_FlashMessages");
        }

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