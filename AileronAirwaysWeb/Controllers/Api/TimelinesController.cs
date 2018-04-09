using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/timelines")]
    public class TimelinesController : Controller
    {
        private readonly TimelineRepository _repo;

        public TimelinesController(TimelineRepository repo)
        {
            _repo = repo;
        }

        // GET: api/timelines
        [HttpGet]
        public IActionResult Get()
        {
            var timelines = _repo.Timelines.OrderByDescending(t => t.CreationTimeStamp).Select(t => new TimelineViewModel
            {
                Id = t.Id,
                Title = t.Title,
                CreationTimeStamp = t.CreationTimeStamp,
                IsDeleted = t.IsDeleted,
                EventsCount = t.EventsCount
            }).ToList();

            return Ok(timelines);
        }

        // GET: api/timelines/abc123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var timeline = _repo.GetTimelineWithEvents(id);

            return Ok(new TimelineViewModel
            {
                Id = timeline.Id,
                Title = timeline.Title,
                CreationTimeStamp = timeline.CreationTimeStamp,
                IsDeleted = timeline.IsDeleted
            });
        }

        // POST: api/timelines
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TimelineViewModel value)
        {
            var timeline = await _repo.CreateTimelineAsync(value.Title);

            return Ok(timeline);
        }

        // PUT: api/timelines/abc123
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]TimelineViewModel value)
        {
            var timeline = _repo.GetTimelineWithEvents(id);
            timeline.Title = value.Title;
            await _repo.UpdateTimelineAsync(timeline);

            return Ok(new TimelineViewModel
            {
                Id = timeline.Id,
                Title = timeline.Title,
                CreationTimeStamp = timeline.CreationTimeStamp,
                IsDeleted = timeline.IsDeleted
            });
        }

        // DELETE: api/timelines/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repo.DeleteTimelineAsync(id);

            return Ok();
        }

        [HttpGet("offline")]
        public async Task<IActionResult> Offline()
        {
            bool offline = await _repo.IsOfflineAsync();

            Debug.WriteLine($"API Offline: {offline}");

            return Ok(new
            {
                offline
            });
        }
    }
}
