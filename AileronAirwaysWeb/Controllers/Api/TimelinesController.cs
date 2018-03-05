using Echelon.TimelineApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/timelines")]
    public class TimelinesController : Controller
    {
        private readonly ITimelineService _api;

        public TimelinesController(ITimelineService api)
        {
            _api = api;
        }

        // TODO: remove this and other VMs once they've been done properly.
        public class TimelineViewModel
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string CreationTimeStamp { get; set; }
            public bool IsDeleted { get; set; }
        }

        // GET: api/timelines
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var timelines = (await Timeline.GetTimelinesAsync(_api))
                .OrderBy(t => t.CreationTimeStamp)
                .Select(t => new TimelineViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CreationTimeStamp = t.CreationTimeStamp.ToShortDateString() + " " + t.CreationTimeStamp.ToShortTimeString(),
                    IsDeleted = t.IsDeleted
                }).ToList();

            return Ok(timelines);
        }

        // GET: api/timelines/abc123
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var timeline = await Timeline.GetTimelineAsync(_api, id);

            return Ok(timeline);
        }

        // POST: api/timelines
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Timeline value)
        {
            var timeline = await Timeline.CreateAsync(_api, value.Title);

            return Ok(timeline);
        }

        // PUT: api/timelines/abc123
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]Timeline value)
        {
            var timeline = await Timeline.GetTimelineAsync(_api, id);

            timeline.Title = value.Title;
            await timeline.EditTitleAsync(_api);

            return Ok(timeline);
        }

        // DELETE: api/timelines/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var timeline = await Timeline.GetTimelineAsync(_api, id);
            await timeline.DeleteAsync(_api);
            return Ok();
        }
    }
}
