using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelinesController : Controller
    {
        private readonly ICachedTimelineService _api;
        private readonly IFlashService _flash;

        public TimelinesController(ICachedTimelineService api, IFlashService flash)
        {
            _api = api;
            _flash = flash;
        }

        // GET: Timelines
        public ActionResult Index()
        {
            // Timelines are loaded through JSON
            return View();
        }

        // GET: Timelines/Details/5
        public async Task<ActionResult> Details(string id)
        {
            Timeline timeline = await CachedTimeline.CacheGetTimelineAsync(_api, id);

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
        public async Task<ActionResult> Create([Bind("Title")] TimelineViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Timeline timeline = await CachedTimeline.CacheCreateAsync(_api, vm.Title);

                _flash.Message("Timeline created!");

                // On success we return OK followed by the ID of the timeline just created.
                return Ok("OK " + timeline.Id);
            }

            // Error - return view with validation errors showing.
            return PartialView(vm);
        }

        // GET: Timelines/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Timeline timeline = await CachedTimeline.CacheGetTimelineAsync(_api, id);

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
        public async Task<ActionResult> Edit(string id, [Bind("Title")] TimelineViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Timeline timeline = await CachedTimeline.CacheGetTimelineAsync(_api, id);
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
            Timeline timeline = await CachedTimeline.CacheGetTimelineAsync(_api, id);

            return View(new TimelineViewModel
            {
                Id = timeline.Id,
                CreationTimeStamp = timeline.CreationTimeStamp,
                Title = timeline.Title
            });
        }

        // POST: Timelines/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            Timeline timeline = await CachedTimeline.CacheGetTimelineAsync(_api, id);

            await timeline.DeleteAsync(_api);

            _flash.Message("Timeline has been deleted!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}