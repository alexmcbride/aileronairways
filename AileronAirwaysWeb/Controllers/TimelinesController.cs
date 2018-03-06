using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AileronAirwaysWeb.ViewModels.TimelineViewModels;


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
            var createVM = new CreateViewModel();

            return PartialView(createVM);
        }

        // POST: Timelines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel VM)
        {
            try
            {
                Timeline tLine = await Timeline.CreateAsync(_api, VM.Title);
                tLine.Title = VM.Title;

                _flash.Message("Timeline created!");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return PartialView();
            }
        }

        // GET: Timelines/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Timeline t = await Timeline.GetTimelineAsync(_api, id);
            
            var editVM = new EditViewModel
            {
                Title = t.Title,
                ID = t.Id
            };
            return PartialView(editVM);
        }

        // POST: Timelines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditViewModel VM)
        {
            try
            {
                Timeline tline = await Timeline.GetTimelineAsync(_api, id);
                tline.Title = VM.Title;

                await tline.EditTitleAsync(_api);

                _flash.Message("Timeline has been edited!");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return PartialView();
            }
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