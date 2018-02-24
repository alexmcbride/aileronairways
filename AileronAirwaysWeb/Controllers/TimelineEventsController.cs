using Echelon.TimelineApi;
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
        private readonly ITimelineService _api;

        public TimelineEventsController(ITimelineService api)
        {
            _api = api;
        }

        // GET: Timelines
        public async Task<ActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            { id = (RouteData.Values["id"]).ToString(); }

            TempData["TimelineId"] = id;

            // Get the timeline.
            Timeline timeline = await Timeline.GetTimelineAsync(_api, id);
            ViewBag.TimelineTitle = timeline.Title;

            IList<LinkedEvent> linkedEvents = await TimelineEvent.GetLinkedEventsAsync(_api, id);

            // Execute list of tasks in one go, which is faster.
            var tasks = linkedEvents.Select(e => TimelineEvent.GetTimelineEventAsync(_api, e.TimelineEventId));
            var timelineEvents = (await Task.WhenAll(tasks))
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.EventDateTime)
                .ToList();

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
            string timelineId;
            if (TempData.ContainsKey("TimelineId"))
            {
                timelineId = TempData["TimelineId"].ToString();
                TempData["TimelineId"] = timelineId;
                ViewBag.TimelineId = timelineId;

                // Create new blank timeline and set default values
                TimelineEvent timelineEvent = new TimelineEvent();
                timelineEvent.EventDateTime = DateTime.Now;

                return View(timelineEvent);
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

            await evt.LinkEventAsync(_api, timelineId);

            return RedirectToAction(/*returnUrl*/nameof(Index), new { id = timelineId });
        }

        //GET: Timelines/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            string timelineId;
            if (TempData.ContainsKey("TimelineId"))
            {
                timelineId = (TempData["TimelineId"]).ToString();
                TempData["TimelineId"] = timelineId;

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
        public async Task<ActionResult> Edit(string id, IFormCollection collection)
        {
            var date = DateTime.Parse(Request.Form["EventDateTime"]);

            TimelineEvent evt = await TimelineEvent.GetTimelineEventAsync(_api, id);
            evt.Title = Request.Form["Title"];
            evt.Description = Request.Form["Description"];
            evt.EventDateTime = date;
            evt.Location = Request.Form["Location"];

            await evt.EditAsync(_api);

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