using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelinesController : Controller
    {
        private readonly ITimelineService _api;

        public TimelinesController(ITimelineService api)
        {
            _api = api;
        }

        // GET: Timelines
        public async Task<ActionResult> Index()
        {
            IList<Timeline> timelines = await Timeline.GetTimelinesAsync(_api);

            return View(timelines);
        }

        // GET: Timelines/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Timelines/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
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