using Echelon.TimelineApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/events")]
    public class TimelineEventsController : Controller
    {
        private readonly ITimelineService _api;

        public TimelineEventsController(ITimelineService api)
        {
            _api = api;
        }

        // GET: api/events/all/abc123
        [HttpGet("all/{timelineId}")]
        public async Task<IActionResult> GetAll(string timelineId)
        {
            var links = await TimelineEvent.GetEventsAsync(_api, timelineId);
            var tasks = links.Select(l => TimelineEvent.GetEventAsync(_api, l.TimelineEventId));
            var events = (await Task.WhenAll(tasks)).Where(e => !e.IsDeleted).OrderBy(e => e.EventDateTime);

            return Ok(events);
        }

        // GET: api/events/abc123
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var @event = await TimelineEvent.GetEventAsync(_api, id);

            return Ok(@event);
        }

        public class TimelineEventViewModel
        {
            public string Id { get; set; }
            public string TimelineId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            [JsonConverter(typeof(CustomDateTimeConverter))]
            public DateTime EventDateTime { get; set; }
            public string Location { get; set; }
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TimelineEventViewModel value)
        {
            var @event = await TimelineEvent.CreateAsync(_api, value.Title, value.Description, value.EventDateTime, value.Location);

            await @event.LinkEventAsync(_api, value.TimelineId);

            return Ok(@event);
        }

        // PUT: api/events/abc123
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]TimelineEvent value)
        {
            // TODO: add validation for JSON stuff.
            var @event = await TimelineEvent.GetEventAsync(_api, id);
            @event.Title = value.Title;
            @event.Description = value.Description;
            @event.EventDateTime = value.EventDateTime;
            @event.Location = value.Location;
            await @event.EditAsync(_api);

            return Ok(@event);
        }

        // DELETE: api/events/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var @event = await TimelineEvent.GetEventAsync(_api, id);
            await @event.DeleteAsync(_api);
            return Ok();
        }
    }
}
