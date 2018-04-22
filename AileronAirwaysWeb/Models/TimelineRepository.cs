using AileronAirwaysWeb.Data;
using AileronAirwaysWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// Class following repository pattern that encapsulates both connection with the API and the SQL Server cache.
    /// </summary>
    public class TimelineRepository
    {
        private const int OfflineCacheMinutes = 1; // Minutes to stop offline cache data.

        private readonly ITimelineService _api;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

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

        public TimelineRepository(ITimelineService api, ApplicationDbContext context, IConfiguration config)
        {
            _api = api;
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Initializes the database, downloads all timelines, events, and attachments, and inserts them into the database.
        /// </summary>
        public void Initialize()
        {
            Debug.WriteLine("TimelineRepository: dropping and recreating DB");
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            Debug.WriteLine("TimelineRepository: getting all timelines and events");
            var timelines = Timeline.GetAllTimelinesAndEvents(_api);

            _context.AddRange(timelines);
            _context.SaveChanges();

            Debug.WriteLine("TimelineRepository: done");
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

        /// <summary>
        /// Gets next event in the timeline after the one specified.
        /// </summary>
        public Task<TimelineEvent> GetNextEventAsync(TimelineEvent @event)
        {
            return _context.TimelineEvents
                .OrderBy(t => t.EventDateTime)
                .Where(e => e.EventDateTime > @event.EventDateTime)
                .Where(e => e.TimelineId == @event.TimelineId)
                .FirstOrDefaultAsync();
        }        
        
        /// <summary>
        /// Gets previous event in the timeline before the one specified.
        /// </summary>
        public Task<TimelineEvent> GetPreviousEventAsync(TimelineEvent @event)
        {
            return _context.TimelineEvents
                .OrderBy(t => t.EventDateTime)
                .Where(e => e.EventDateTime < @event.EventDateTime)
                .Where(e => e.TimelineId == @event.TimelineId)
                .LastOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new timeline.
        /// </summary>
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

        /// <summary>
        /// Edits the specified timeline event, saving it to API and cache.
        /// </summary>
        public async Task EditTimelineEventAsync(TimelineEvent evt)
        {
            await evt.EditAsync(_api);
            _context.Entry(evt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Edits the specified timeline event location, saving it to API and cache.
        /// </summary>
        public async Task EditEventLocationAsync(TimelineEvent evt)
        {
            await evt.EditLocationAsync(_api);
            _context.Entry(evt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the specified timeline event from API and cache.
        /// </summary>
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

        /// <summary>
        /// Gets the attachment from the cache with the specified ID.
        /// </summary>
        public Attachment GetAttachment(string attachmentId)
        {
            return _context.Attachments.Find(attachmentId);
        }

        /// <summary>
        /// Gets all the attachments from the cache for the specified event ID.
        /// </summary>
        public IQueryable<Attachment> GetAttachments(string eventId)
        {
            return _context.Attachments.Where(a => a.TimelineEventId == eventId);
        }

        /// <summary>
        /// Creates the attachment on the cache and the API.
        /// </summary>
        /// <param name="eventId">The ID of the event to attach the attachment to</param>
        /// <param name="fileName">The path of the uploaded file on the server</param>
        /// <param name="stream">A stream object holding the file data</param>
        /// <returns>An object containing details about the uploaded attachment</returns>
        public async Task<Attachment> CreateAttachmentAsync(string eventId, string fileName, Stream stream)
        {
            var attachment = await Attachment.CreateAndUploadAsync(_api, eventId, Path.GetFileName(fileName), stream);

            // Set relation and update event attachment counters.
            var @event = await _context.TimelineEvents.FindAsync(eventId);
            attachment.TimelineEvent = @event;

            // Increment correct counter value.
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

        /// <summary>
        /// Downloads the attachment with the specified ID.
        /// </summary>
        public async Task<Attachment> DownloadAttachmentAsync(string attachmentId)
        {
            var attachment = GetAttachment(attachmentId);
            await attachment.DownloadOrCacheAsync(_api);
            return attachment;
        }

        /// <summary>
        /// Deletes the attachment with the specified ID.
        /// </summary>
        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            var attachment = await Attachment.GetAttachmentAsync(_api, attachmentId);
            await attachment.DeleteAsync(_api);

            // Remove from DB and update counters
            _context.Attachments.Remove(attachment);
            var @event = await _context.TimelineEvents.FindAsync(attachment.TimelineEventId);

            // Decrement the counter values.
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

        /// <summary>
        /// Edit the description of a timeline event, saving it to API and cache.
        /// </summary>
        public async Task EditDescriptionAsync(TimelineEvent @event)
        {
            await @event.EditDescriptionAsync(_api);
            _context.Entry(@event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds an API event, used to track issues with the API over time.
        /// </summary>
        private Task AddApiEventAsync(string name)
        {
            _context.ApiEvents.Add(new ApiEvent
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Timestamp = DateTime.Now
            });
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds the most recent API event in the database.
        /// </summary>
        private Task<ApiEvent> FindRecentOfflineEventAsync()
        {
            var span = TimeSpan.FromMinutes(OfflineCacheMinutes);
            var time = DateTime.Now.Subtract(span);
            return _context.ApiEvents.Where(a => a.Timestamp > time)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if the API is offline, but trying to download some data and then catching any errors.
        /// </summary>
        public async Task<bool> IsOfflineAsync()
        {
            if (_config.GetValue<bool>("TestReadonlyMode"))
            {
                Debug.WriteLine("Debug offline mode");
                return true;
            }

            try
            {
                var apiEvent = await FindRecentOfflineEventAsync();
                if (apiEvent != null && apiEvent.Name == "Offline")
                {
                    Debug.WriteLine("Using cached offline mode");
                    return true;
                }

                // Try and get a timeline.
                await Timeline.GetTimelinesAsync(_api);
                return false;
            }
            catch (WebException)
            {
                Debug.WriteLine("Caching offline");
                await AddApiEventAsync("Offline");
                return true;
            }
        }
    }
}
