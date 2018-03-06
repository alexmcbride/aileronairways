using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelinesController : Controller
    {
        private readonly ITimelineService _api;
        private readonly IFlashService _flash;

        public TimelinesController(ITimelineService api, IFlashService flash)
        {
            _api = api;
            _flash = flash;
        }

        // GET: Timelines
        public async Task<ActionResult> Index()
        {
            IList<Timeline> timelines = (await Timeline.GetTimelinesAsync(_api))
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreationTimeStamp)
                .ToList();

            return View(timelines);
        }

        // GET: Timelines/Details/5
        public async Task<ActionResult> Details(string id)
        {
            Timeline timeline = await Timeline.GetTimelineAsync(_api, id);

            return View(timeline);
        }

        // GET: Timelines/Create
        public ActionResult Create()
        {
            var vm = new TimelineViewModel();

            return PartialView(vm);
        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TimelineViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Timeline timeline = await Timeline.CreateAsync(_api, vm.Title);
                timeline.Title = vm.Title;

                _flash.Message("Timeline created!");

                return Ok("OK " + timeline.Id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Timeline timeline = await Timeline.GetTimelineAsync(_api, id);

            var vm = new TimelineViewModel
            {
                Title = timeline.Title,
                Id = timeline.Id
            };
            return PartialView(vm);
        }

        // POST: Timelines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, TimelineViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Timeline timeline = await Timeline.GetTimelineAsync(_api, id);
                timeline.Title = vm.Title;
                await timeline.EditTitleAsync(_api);

                _flash.Message("Timeline has been edited!");

                return Ok("OK " + id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Timeline tline = await Timeline.GetTimelineAsync(_api, id.ToString());
            await tline.DeleteAsync(_api);

            _flash.Message("Timeline has been deleted!");

            return RedirectToAction(nameof(Index));
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}