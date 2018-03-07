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
                await _context.AddRangeAsync(timelines);
                await _context.SaveChangesAsync();
            }
        }

        public Attachment GetAttachment(string attachmentId)
        {
            return _context.Attachments.Find(attachmentId);
        }

        public Timeline GetTimeline(string id)
        {
            return _context.Timelines.Include(t => t.TimelineEvents).SingleOrDefault(t => t.Id == id);
        }

        public async Task<Attachment> DownloadAttachmentAsync(string attachmentId)
        {
            var attachment = GetAttachment(attachmentId);
            await attachment.DownloadOrCacheAsync(_api);
            return attachment;
        }

        public async Task<Timeline> CreateTimelineAsync(string title)
        {
            var timeline = await Timeline.CreateAsync(_api, title);
            await _context.Timelines.AddAsync(timeline);
            await _context.SaveChangesAsync();
            return timeline;
        }

        public async Task<Attachment> CreateAttachmentAsync(string eventId, string fileName, Stream stream)
        {
            var attachment = await Attachment.CreateAndUploadAsync(_api, eventId, fileName, stream);

            attachment.TimelineEventId = eventId;
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task UpdateTimelineAsync(Timeline timeline)
        {
            await timeline.EditTitleAsync(_api);
            _context.Entry(timeline).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimelineAsync(string id)
        {
            var timeline = GetTimeline(id);
            await timeline.DeleteAsync(_api);
            _context.Timelines.Remove(timeline);
            await _context.SaveChangesAsync();
        }

        public TimelineEvent GetTimelineEvent(string id)
        {
            return _context.TimelineEvents.Include(e => e.Attachments).SingleOrDefault(e => e.Id == id);
        }

        public async Task<TimelineEvent> CreateTimelineEventAsync(string title, string description, DateTime eventDateTime, string location, string timelineId)
        {
            var timelineEvent = await TimelineEvent.CreateAndLinkAsync(_api, title, description, eventDateTime, location, timelineId);
            timelineEvent.TimelineId = timelineId;
            await _context.TimelineEvents.AddAsync(timelineEvent);
            await _context.SaveChangesAsync();
            return timelineEvent;
        }

        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);
            await attachment.DeleteAsync(_api);

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task EditTimelineEventAsync(TimelineEvent evt)
        {
            await evt.EditAsync(_api);
            _context.Entry(evt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimelineEventAsync(TimelineEvent evt)
        {
            await TimelineEvent.UnlinkAndDeleteAsync(_api, evt.TimelineId, evt.Id);
            _context.TimelineEvents.Remove(evt);
            await _context.SaveChangesAsync();
        }

        
    }
}
