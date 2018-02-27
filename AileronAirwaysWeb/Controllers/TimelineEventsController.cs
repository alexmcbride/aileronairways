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
            //TempData["TimelineId"] = id;
            HttpContext.Session.SetString("TimelineId",id);

            // Get the timeline.
            if (id.Equals(null))
            { id = (RouteData.Values["id"]).ToString();
            }

            var timeline = await GetTimeline(id);
            
            //get linked events first
            IList<LinkedEvent> timelinelinkedEvents = await TimelineEvent.GetEventsAsync(_api, id);
            //based on the id get timeline linked events
                var tasks = timelinelinkedEvents.Select(e => TimelineEvent.GetEventAsync(_api, e.TimelineEventId));
            var timelineEvents = (await Task.WhenAll(tasks))
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.EventDateTime)
                .ToList();

            return View(timelineEvents);
        }

        private async Task<TimelineWithEvents> GetTimeline(string id)
        {
            var timeslines = await Timeline.GetAllTimelinesAndEventsAsync(_api);
            return timeslines.SingleOrDefault(t => t.Id == id);
        }

        // GET: Timelines/Details/5
        public async Task<ActionResult> Details(string id)
        {

            TimelineEvent Event = await TimelineEvent.GetEventAsync(_api, id);
            //string timelineId = (TempData["TimelineId"]).ToString();
            //TempData["TimelineId"] = timelineId;
            var timelineId = HttpContext.Session.GetString("TimelineId");

            ViewBag.TimelineId = timelineId;
            return View(Event);
        }

        // GET: Timelines/Create
        public ActionResult Create()
        {


            //if (TempData.ContainsKey("TimelineId"))

            //{string timelineId;
            //timelineId = TempData["TimelineId"].ToString();
            //TempData["TimelineId"] = timelineId;
            string timelineId = HttpContext.Session.GetString("TimelineId");
            ViewBag.TimelineId = timelineId;

            // Create new blank timeline and set default values
            TimelineEvent timelineEvent = new TimelineEvent
            {
                //EventDateTime = DateTime.Now
            };

            return View(timelineEvent);
            //}
            //else
            //{

            //    return RedirectToAction("Index", "Timelines");
            //}
        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection/*, string returnUrl = null*/)
        {
            //string timelineId = (TempData["TimelineId"]).ToString();
            string timelineId = HttpContext.Session.GetString("TimelineId");
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


            //if (TempData.ContainsKey("TimelineId"))
            //{   string timelineId;
            //timelineId = (TempData["TimelineId"]).ToString();
            //TempData["TimelineId"] = timelineId;
            

               TimelineEvent timelineEvent = await TimelineEvent.GetEventAsync(_api, id);


                return View(timelineEvent);
            //}
            //else
            //{
                //return RedirectToAction("Details", "TimelineEvents", new { id = id });
            //}
        }

        //POST: Timelines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, IFormCollection collection)
        {

            if (HttpContext.Session.Keys.Contains("TimelineId"))
            {
                var date = DateTime.Parse(Request.Form["EventDateTime"]);
                string timelineId = HttpContext.Session.GetString("TimelineId");
                TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, id);
            evt.Id = id;


            evt.Title = Request.Form["Title"];
            evt.Description = Request.Form["Description"];
            evt.EventDateTime = date;
            evt.Location = Request.Form["Location"];

            await evt.EditAsync(_api);


            //if (TempData.ContainsKey("TimelineId"))
            //{
            //    string timelineId;
            //    timelineId = (TempData["TimelineId"]).ToString();
                //TempData["TimelineId"] = timelineId;

                return RedirectToAction("Details", "TimelineEvents", new { Id = id });
            }
            else
            {
                return RedirectToAction("Details", "TimelineEvent", new { Id = id });
            }
        }


        // GET: Timelines/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            //string timelineId = (TempData["TimelineId"]).ToString();

            TimelineEvent evt = await TimelineEvent.GetEventAsync(_api, id);
            await evt.DeleteAsync(_api);
            if (HttpContext.Session.Keys.Contains("TimelineId"))
            {
                //if (TempData.ContainsKey("TimelineId"))
                //{
                //    string timelineId;
                //    timelineId = (TempData["TimelineId"]).ToString();
                //TempData["TimelineId"] = timelineId;
                string timelineId = HttpContext.Session.GetString("TimelineId");
                return RedirectToAction("Index", "TimelineEvents", new { id = timelineId });
            }
            else
            {
                return RedirectToAction("Index", "Timeline");
            }
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