using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
                IsDeleted = t.IsDeleted
            }).ToList();

            return Ok(timelines);
        }

        // GET: api/timelines/abc123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var timeline = _repo.GetTimeline(id);

            return Ok(timeline);
        }

        // POST: api/timelines
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Timeline value)
        {
            var timeline = await _repo.CreateTimelineAsync(value.Title);

            return Ok(timeline);
        }

        // PUT: api/timelines/abc123
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]Timeline value)
        {
            var timeline = _repo.GetTimeline(id);
            timeline.Title = value.Title;
            await _repo.UpdateTimelineAsync(timeline);

            return Ok(timeline);
        }

        // DELETE: api/timelines/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repo.DeleteTimelineAsync(id);

            return Ok();
        }
    }
}
