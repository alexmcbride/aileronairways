﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelineEventsController : Controller
    {

        private readonly ITimelineService _api;

        public TimelineEventsController(ITimelineService api)
        {
            _api = api;
        }

        // GET: Timelines
        public async Task<ActionResult> Index(string id)
        {
            if (id.Equals(null))
            { id = (RouteData.Values["id"]).ToString(); }

            IList<LinkedEvent> linkedEvents = await TimelineEvent.GetLinkedEventsAsync(_api, id);
            List<TimelineEvent> timelineEvents = new List<TimelineEvent>();
            foreach (var item in linkedEvents)
            {
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(_api, item.TimelineEventId);
                if (timelineEvent.IsDeleted == false)
                {
                    timelineEvents.Add(timelineEvent);
                }
            }
            TempData["TimelineId"] = id;

            // Sort events 
            timelineEvents = timelineEvents.OrderByDescending(e => e.EventDateTime).ToList();

            return View(timelineEvents);
        }

        // GET: Timelines/Details/5
        public async Task<ActionResult> Details(string id)
        {

            TimelineEvent Event = await TimelineEvent.GetTimelineEventAsync(_api, id);
            string timelineId = (TempData["TimelineId"]).ToString();
            TempData["TimelineId"] = timelineId;
            ViewBag.TimelineId = timelineId;
            return View(Event);
        }

        // GET: Timelines/Create
        public ActionResult Create()
        {
            string TimelineId;
            if (TempData.ContainsKey("TimelineId"))
            {
                TimelineId = TempData["TimelineId"].ToString();
                TempData["TimelineId"] = TimelineId;
                return View();
            }
            else
            {

                return RedirectToAction("Index", "Timelines");
            }

        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection/*, string returnUrl = null*/)
        { 
            string timelineId = (TempData["TimelineId"]).ToString();
            DateTime date = DateTime.Parse(Request.Form["EventDateTime"]);

            TimelineEvent evt = await TimelineEvent.CreateAsync(_api,
                Request.Form["Title"],
                Request.Form["Description"],
                date,
                Request.Form["Location"]);

            Timeline timeline = await Timeline.GetTimelineAsync(_api, timelineId);
            await timeline.LinkEventAsync(_api, evt);

            return RedirectToAction(/*returnUrl*/nameof(Index), new { id = timelineId });
        }

        //GET: Timelines/Edit/5
        public async Task<ActionResult> EditTitleDateDescription(string id)
        {
            string TimelineId;
            if (TempData.ContainsKey("TimelineId"))
            {
                TimelineId = (TempData["TimelineId"]).ToString();
                TempData["TimelineId"] = TimelineId;
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(_api, id);
                return View(timelineEvent);
            }
            else
            {
                return RedirectToAction("Details", "TimelineEvents");
            }
        }

        //POST: Timelines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTitleDateDescription(string id, IFormCollection collection)
        {
            var date = DateTime.Parse(Request.Form["EventDateTime"]);

            TimelineEvent evt = await TimelineEvent.GetTimelineEventAsync(_api, id);
            evt.Title = Request.Form["Title"];
            evt.Description = Request.Form["Description"];
            evt.EventDateTime = date;

            await evt.EditTitleAsync(_api);
            await evt.EditEventDateTimeAsync(_api);
            await evt.EditDescriptionAsync(_api);

            if (TempData.ContainsKey("TimelineId"))
            {
                string timelineId;
                timelineId = (TempData["TimelineId"]).ToString();
                TempData["TimelineId"] = timelineId;
                return RedirectToAction("Details", "TimelineEvents", new { id = evt.Id });
            }
            else
            {
                return RedirectToAction("Details", "TimelineEvent", new { id = evt.Id });
            }
        }


        // GET: Timelines/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            string timelineId = (TempData["TimelineId"]).ToString();

            TimelineEvent evt = await TimelineEvent.GetTimelineEventAsync(_api, id);
            await evt.DeleteAsync(_api);
            TempData["TimelineId"] = timelineId;
            return RedirectToAction("Index", "TimelineEvents", new { id = timelineId });
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