using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    public class TimelineRepository
    {
        private readonly ITimelineService _api;
        private readonly ApplicationDbContext _context;

        public IQueryable<Timeline> Timelines
        {
            get { return _context.Timelines; }
        }

        public IQueryable<TimelineEvent> TimelineEvents
        {
            get { return _context.TimelineEvents; }
        }

        public IQueryable<Attachment> Attachments
        {
            get { return _context.Attachments; }
        }

        public TimelineRepository(ITimelineService api, ApplicationDbContext context)
        {
            _api = api;
            _context = context;
        }

        public async Task InitializeAsync()
        {
            // If no timelines in DB, then populate from API.
            if (!await _context.Timelines.AnyAsync())
            {
                var timelines = await Timeline.GetAllTimelinesAndEventsAsync(_api);

                foreach (var timeline in timelines)
                {
                    timeline.UpdateCalculatedColumns();
                }

                await _context.AddRangeAsync(timelines);
                await _context.SaveChangesAsync();
            }
        }

        public Timeline GetTimeline(string id)
        {
            return _context.Timelines.SingleOrDefault(t => t.Id == id);
        }

        public Timeline GetTimelineWithEvents(string id)
        {
            return _context.Timelines.Include(t => t.TimelineEvents).SingleOrDefault(t => t.Id == id);
        }

        public async Task<Timeline> CreateTimelineAsync(string title)
        {
            var timeline = await Timeline.CreateAsync(_api, title);
            await _context.Timelines.AddAsync(timeline);
            await _context.SaveChangesAsync();
            return timeline;
        }

        public async Task UpdateTimelineAsync(Timeline timeline)
        {
            await timeline.EditTitleAsync(_api);
            _context.Entry(timeline).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimelineAsync(string id)
        {
            var timeline = GetTimelineWithEvents(id);
            await timeline.DeleteAsync(_api);
            _context.Timelines.Remove(timeline);
            await _context.SaveChangesAsync();
        }

        public TimelineEvent GetTimelineEvent(string id)
        {
            return _context.TimelineEvents.SingleOrDefault(e => e.Id == id);
        }

        public TimelineEvent GetTimelineEventWithAttachments(string id)
        {
            return _context.TimelineEvents.Include(e => e.Attachments).SingleOrDefault(e => e.Id == id);
        }

        public Task<TimelineEvent> GetNextEventAsync(TimelineEvent @event)
        {
            return _context.TimelineEvents
                .OrderBy(t => t.EventDateTime)
                .Where(e => e.EventDateTime > @event.EventDateTime)
                .Where(e => e.TimelineId == @event.TimelineId)
                .FirstOrDefaultAsync();
        }

        public Task<TimelineEvent> GetPreviousEventAsync(TimelineEvent @event)
        {
            return _context.TimelineEvents
                .OrderBy(t => t.EventDateTime)
                .Where(e => e.EventDateTime < @event.EventDateTime)
                .Where(e => e.TimelineId == @event.TimelineId)
                .LastOrDefaultAsync();
        }

        public async Task<TimelineEvent> CreateTimelineEventAsync(string title, string description, DateTime eventDateTime, string location, string timelineId)
        {
            var timelineEvent = await TimelineEvent.CreateAndLinkAsync(_api, title, description, eventDateTime, location, timelineId);
            timelineEvent.TimelineId = timelineId;
            await _context.TimelineEvents.AddAsync(timelineEvent);

            // Increment event count
            Timeline timeline = GetTimeline(timelineId);
            timeline.EventsCount++;
            _context.Entry(timeline).State = EntityState.Modified;
    
            // Save
            await _context.SaveChangesAsync();
            return timelineEvent;
        }

        public async Task EditTimelineEventAsync(TimelineEvent evt)
        {
            await evt.EditAsync(_api);
            _context.Entry(evt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task EditEventLocationAsync(TimelineEvent evt)
        {
            await evt.EditLocationAsync(_api);
            _context.Entry(evt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimelineEventAsync(TimelineEvent evt)
        {
            await TimelineEvent.UnlinkAndDeleteAsync(_api, evt.TimelineId, evt.Id);
            _context.TimelineEvents.Remove(evt);

            // Deincrement event count
            var timeline = GetTimeline(evt.TimelineId);
            timeline.EventsCount--;
            _context.Entry(timeline).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public Attachment GetAttachment(string attachmentId)
        {
            return _context.Attachments.Find(attachmentId);
        }

        public IQueryable<Attachment> GetAttachments(string eventId)
        {
            return _context.Attachments.Where(a => a.TimelineEventId == eventId);
        }

        public async Task<Attachment> CreateAttachmentAsync(string eventId, string fileName, Stream stream)
        {
            var attachment = await Attachment.CreateAndUploadAsync(_api, eventId, fileName, stream);

            // Set relation and update event attachment counters.
            var @event = await _context.TimelineEvents.FindAsync(eventId);
            attachment.TimelineEvent = @event;

            if (attachment.IsImage)
            {
                @event.AttachmentImagesCount++;
            }
            else
            {
                @event.AttachmentFilesCount++;
            }

            _context.Entry(@event).State = EntityState.Modified;

            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<Attachment> DownloadAttachmentAsync(string attachmentId)
        {
            var attachment = GetAttachment(attachmentId);
            await attachment.DownloadOrCacheAsync(_api);
            return attachment;
        }

        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);
            await attachment.DeleteAsync(_api);

            // Remove from DB and update counters
            _context.Attachments.Remove(attachment);
            var @event = await _context.TimelineEvents.FindAsync(attachment.TimelineEventId);
            if (attachment.IsImage)
            {
                @event.AttachmentImagesCount--;
            }
            else
            {
                @event.AttachmentFilesCount--;
            }
            _context.Entry(@event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task EditDescriptionAsync(TimelineEvent @event)
        {
            await @event.EditDescriptionAsync(_api);
            _context.Entry(@event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
