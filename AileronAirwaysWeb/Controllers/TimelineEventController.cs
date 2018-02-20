using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelineEventController : Controller
    {

        private readonly ITimelineService _api;

        public TimelineEventController(ITimelineService api)
        {
            _api = api;
        }

        // GET: Timelines
        public async Task<ActionResult> Index(string id)
        {
            Timeline timeline = await Timeline.GetTimelineAsync(_api, id);
            IList<LinkedEvent> linkedEvents = await timeline.GetEventsAsync(_api);
            List<TimelineEvent> timelineEvents = new List<TimelineEvent>();
            foreach (var item in linkedEvents)
            {
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(_api, item.TimelineEventId);
                timelineEvents.Add(timelineEvent);
            }

            return View(timelineEvents);
        }

        // GET: Timelines/Details/5
        public async Task<ActionResult> Details(string id)
        {

            TimelineEvent Event = await TimelineEvent.GetTimelineEventAsync(_api, id);
            TempData["TimelineId"] = id;
            return View(Event);
        }

        // GET: Timelines/Create
        public ActionResult Create()
        {
            string TimelineId = (TempData["TimelineId"]).ToString();
            TempData["TimelineId"] = TimelineId;

            return View();
        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                TimelineEvent evt = await TimelineEvent.CreateAsync(_api, 
                    Request.Form["Title"], 
                    Request.Form["Description"], 
                    Convert.ToDateTime(Request.Form["EvetDateTime"]), 
                    Request.Form["Location"]);

                string timelineId = (TempData["TimelineId"]).ToString();
                await evt.LinkEventAsync(_api, timelineId);

                return RedirectToAction(returnUrl/*nameof(Index)*/);
            }
            catch
            {
                return View();
            }
        }

        // GET: Timelines/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Timelines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Timelines/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Timelines/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}