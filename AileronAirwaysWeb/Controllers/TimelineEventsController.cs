using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using AileronAirwaysWeb.ViewModels.EventsViewModel;


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
            ViewBag.TimelineId = timelineId;

            // Create new blank timeline and set default values
            TimelineEvent evt = new TimelineEvent();
            evt.EventDateTime = DateTime.Now;
            CreateViewModel CVM = new CreateViewModel
            {
                DateTime = evt.EventDateTime
            };

            return PartialView(CVM);
        }

        [HttpPost("Timelines/{timelineId}/Events/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string timelineId, CreateViewModel CVM)
        {
            if(ModelState.IsValid){ 
            //DateTime date = DateTime.Parse(Request.Form["EventDateTime"]);

            TimelineEvent evt = await TimelineEvent.CreateAsync(_api,
                CVM.Title,
                CVM.Description,
                CVM.DateTime,
                CVM.Location);
            await evt.LinkEventAsync(_api, timelineId);

            _flash.Message($"Event '{evt.Title}' added!");

            ViewBag.TimelineId = timelineId;

            return RedirectToAction(nameof(Index), new { timelineId });
        }
            //return BadRequest(ModelState);
            return View(CVM);
        }

        //GET: Timelines/Edit/5
        [HttpGet("Timelines/{timelineId}/Events/{eventId}/Edit")]
        public async Task<ActionResult> Edit(string timelineId, string eventId)
        {
            TimelineEvent timelineEvent = await TimelineEvent.GetEventAsync(_api, eventId);

            ViewBag.TimelineId = timelineId;

            //view model = model
            //EVM = Edit View Model
            EditViewModel EVM = new EditViewModel
            {
                Title = timelineEvent.Title,
                Description = timelineEvent.Description,
                DateTime = timelineEvent.EventDateTime,
                Location = timelineEvent.Location
            };

            return PartialView(EVM);
        }

        //POST: Timelines/Edit/5
        [HttpPost("Timelines/{timelineId}/Events/{eventId}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string timelineId, string eventId, EditViewModel EVM)
        {
            //var date = DateTime.Parse(Request.Form["EventDateTime"]);
            if (ModelState.IsValid) { 
                TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, eventId);
            evt.Title = EVM.Title;
            evt.Description = EVM.Description;
            evt.EventDateTime = EVM.DateTime;
            evt.Location = EVM.Location;

            await evt.EditAsync(_api);

            _flash.Message($"Event '{evt.Title}' edited!");

            return RedirectToAction("Details", "TimelineEvents", new { timelineId, eventId = evt.Id });
            }
            return BadRequest(ModelState);
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