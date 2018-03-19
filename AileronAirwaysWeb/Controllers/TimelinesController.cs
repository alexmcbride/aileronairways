using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers
{
    public class TimelinesController : Controller
    {
        private readonly IFlashService _flash;
        private readonly TimelineRepository _repo;

        public TimelinesController(TimelineRepository repo, IFlashService flash)
        {
            _flash = flash;
            _repo = repo;
        }

        // GET: Timelines
        public async Task<ActionResult> Index()
        {
            await _repo.InitializeAsync();

            return View();
        }

        // GET: Timelines/Details/5
        public ActionResult Details(string id)
        {
            Timeline timeline = _repo.GetTimelineWithEvents(id);

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
                var timeline = await _repo.CreateTimelineAsync(vm.Title);

                _flash.Message("Timeline created!");

                // On success we return OK followed by the ID of the timeline just created.
                return Ok("OK " + timeline.Id);
            }

            // Error - return view with validation errors showing.
            return PartialView(vm);
        }

        // GET: Timelines/Edit/5
        public ActionResult Edit(string id)
        {
            var timeline = _repo.GetTimelineWithEvents(id);

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
                Timeline timeline = _repo.GetTimelineWithEvents(id);
                timeline.Title = vm.Title;
                await _repo.UpdateTimelineAsync(timeline);

                _flash.Message("Timeline has been edited!");

                return Ok("OK " + id);
            }

            return PartialView(vm);
        }

        // GET: Timelines/Delete/5
        public ActionResult Delete(string id)
        {
            Timeline timeline = _repo.GetTimelineWithEvents(id);

            return PartialView(new TimelineViewModel
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
            await _repo.DeleteTimelineAsync(id);

            _flash.Message("Timeline has been deleted!");

            return Ok("OK " + id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}