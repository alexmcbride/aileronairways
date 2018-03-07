using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Controllers.Api
{
    [Area("api")]
    [Produces("application/json")]
    [Route("api/events")]
    public class TimelineEventsController : Controller
    {
        private readonly TimelineRepository _repo;

        public TimelineEventsController(TimelineRepository repo)
        {
            _repo = repo;
        }

        // GET: api/events/all/abc123
        [HttpGet("all/{timelineId}")]
        public IActionResult GetAll(string timelineId)
        {
            var events = _repo.TimelineEvents
                .Where(e => e.TimelineId == timelineId)
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.EventDateTime)
                .Select(e => new TimelineEventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    TimelineId = e.TimelineId,
                    Description = e.Description,
                    EventDateTime = e.EventDateTime,
                    Location = e.Location,
                    AttachmentFilesCount = e.AttachmentFilesCount,
                    AttachmentImagesCount = e.AttachmentImagesCount
                });

            return Ok(events);
        }

        // GET: api/events/abc123
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var @event = _repo.GetTimelineEvent(id);

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(new TimelineEventViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                TimelineId = @event.TimelineId,
                Description = @event.Description,
                EventDateTime = @event.EventDateTime,
                Location = @event.Location,
                AttachmentFilesCount = @event.AttachmentFilesCount,
                AttachmentImagesCount = @event.AttachmentImagesCount
            });
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TimelineEventViewModel value)
        {
            var @event = await _repo.CreateTimelineEventAsync(value.Title, value.Description, value.EventDateTime, value.Location, value.TimelineId);

            return Ok(new TimelineEventViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                TimelineId = @event.TimelineId,
                Description = @event.Description,
                EventDateTime = @event.EventDateTime,
                Location = @event.Location,
                AttachmentFilesCount = @event.AttachmentFilesCount,
                AttachmentImagesCount = @event.AttachmentImagesCount
            });
        }

        // PUT: api/events/abc123
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]TimelineEvent value)
        {
            // TODO: add validation for JSON stuff.
            var @event = _repo.GetTimelineEvent(id);
            @event.Title = value.Title;
            @event.Description = value.Description;
            @event.EventDateTime = value.EventDateTime;
            @event.Location = value.Location;
            await _repo.EditTimelineEventAsync(@event);

            return Ok(new TimelineEventViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                TimelineId = @event.TimelineId,
                Description = @event.Description,
                EventDateTime = @event.EventDateTime,
                Location = @event.Location,
                AttachmentFilesCount = @event.AttachmentFilesCount,
                AttachmentImagesCount = @event.AttachmentImagesCount
            });
        }

        // DELETE: api/events/abc123
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var @event = _repo.GetTimelineEvent(id);
            await _repo.DeleteTimelineEventAsync(@event);
            return Ok();
        }
    }
}
