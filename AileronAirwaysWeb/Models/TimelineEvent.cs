using AileronAirwaysWeb.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// Class to represent a timeline event.
    /// </summary>
    public class TimelineEvent
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Number of files attached to this event counter.
        /// </summary>
        public int AttachmentFilesCount { get; set; }

        /// <summary>
        /// Number of image files attached counter.
        /// </summary>
        public int AttachmentImagesCount { get; set; }

        /// <summary>
        /// Timeline foreign key.
        /// </summary>
        public string TimelineId { get; set; }

        /// <summary>
        /// Timeline this event is linked to.
        /// </summary>
        [JsonIgnore]
        public virtual Timeline Timeline { get; set; }

        /// <summary>
        /// List of this events attachments.
        /// </summary>
        public virtual List<Attachment> Attachments { get; set; }

        /// <summary>
        /// List of just the image attachments
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Attachment> ImageAttachments
        {
            get { return Attachments.Where(a => a.IsImage); }
        }

        /// <summary>
        /// List of just the ile attachments
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Attachment> FileAttachments
        {
            get { return Attachments.Where(a => !a.IsImage); }
        }

        /// <summary>
        /// Gets or sets the event location in the format "longitude,latitude" e.g. "1.0000,1.0000"
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets if the location propery has been set previously.
        /// </summary>
        public bool HasLocation
        {
            get { return Location != null && Location != "0,0"; }
        }

        /// <summary>
        /// Creates a new event not linked to any timeline.
        /// </summary>
        public static Task<TimelineEvent> CreateAsync(ITimelineService api, string title, string description, DateTime eventDateTime, string location)
        {
            return CreateAsync(api, Guid.NewGuid().ToString(), title, description, eventDateTime, location);
        }

        /// <summary>
        /// Creates a new event not linked to any timeline.
        /// </summary>
        private static async Task<TimelineEvent> CreateAsync(ITimelineService api, string id, string title, string description, DateTime eventDateTime, string location)
        {
            string json = await api.PutJsonAsync("TimelineEvent/Create", new
            {
                TimelineEventId = id,
                Title = title,
                Description = description,
                EventDateTime = eventDateTime.Ticks.ToString(),
                Location = location
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }

        /// <summary>
        /// Creates a new event and links it to the specified timeline.
        /// </summary>
        public static async Task<TimelineEvent> CreateAndLinkAsync(ITimelineService api, string title, string description, DateTime eventDateTime, string location, string timelineId)
        {
            var timelineEvent = await CreateAsync(api, title, description, eventDateTime, location);
            await timelineEvent.LinkEventAsync(api, timelineId);
            return timelineEvent;
        }

        /// <summary>
        /// Saves a change to the title of the event to the API.
        /// </summary>
        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditTitle", new
            {
                TimelineEventId = Id,
                Title
            });
        }

        /// <summary>
        /// Saves a change to the description of the event to the API.
        /// </summary>
        public Task EditDescriptionAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditDescription", new
            {
                TimelineEventId = Id,
                Description
            });
        }

        /// <summary>
        /// Saves a change to the location of the event to the API.
        /// </summary>
        public Task EditLocationAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditLocation", new
            {
                TimelineEventId = Id,
                Location
            });
        }

        /// <summary>
        /// Saves a change to the event datetime of the event to the API.
        /// </summary>
        public Task EditEventDateTimeAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditEventDateTime", new
            {
                TimelineEventId = Id,
                EventDateTime = EventDateTime.Ticks.ToString()
            });
        }

        /// <summary>
        /// Deletes this event from the API.
        /// </summary>
        public Task DeleteAsync(ITimelineService api)
        {
            return DeleteAsync(api, Id);
        }

        /// <summary>
        /// Deletes the event with the specified ID from the API.
        /// </summary>
        public static Task DeleteAsync(ITimelineService api, string timelineEventId)
        {
            return api.PutJsonAsync("TimelineEvent/Delete", new
            {
                TimelineEventId = timelineEventId
            });
        }

        /// <summary>
        /// Gets the event with the specified ID from the API.
        /// </summary>
        public static async Task<TimelineEvent> GetEventAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEvent/GetTimelineEvent", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }

        /// <summary>
        /// Gets all events for the timeline with the specified ID from the API.
        /// </summary>
        public static async Task<IList<LinkedEvent>> GetEventsAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetEvents", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<List<LinkedEvent>>(json);
        }

        /// <summary>
        /// Links this event to the timeline with the specified ID.
        /// </summary>
        public Task LinkEventAsync(ITimelineService api, string timelineId)
        {
            return api.PutJsonAsync("Timeline/LinkEvent", new
            {
                TimelineId = timelineId,
                EventId = Id
            });
        }

        /// <summary>
        /// Unlinks this event from the timeline with the specified ID.
        /// </summary>
        public Task UnlinkEventAsync(ITimelineService api, string timelineId)
        {
            return UnlinkEventAsync(api, timelineId, Id);
        }

        /// <summary>
        /// Unlinks the event with the ID from the timeline with the specified ID.
        /// </summary>
        public static Task UnlinkEventAsync(ITimelineService api, string timelineId, string timelineEventId)
        {
            return api.PutJsonAsync("Timeline/UnlinkEvent", new
            {
                TimelineId = timelineId,
                EventId = timelineEventId
            });
        }

        /// <summary>
        /// Unlinks an event from a timeline and deletes it in a single action.
        /// </summary>
        public static async Task UnlinkAndDeleteAsync(ITimelineService api, string timelineId, string timelineEventId)
        {
            await UnlinkEventAsync(api, timelineId, timelineEventId);
            await DeleteAsync(api, timelineEventId);
        }


        /// <summary>
        /// Edits the event, saving all data to the API.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public Task EditAsync(ITimelineService api)
        {
            // If you perform a create and keep the ID the same then it overwrites the exsting event.
            return CreateAsync(api, Id, Title, Description, EventDateTime, Location);
        }

        /// <summary>
        /// Gets all events from the API.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static async Task<IList<TimelineEvent>> GetAllEventsAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("TimelineEvent/GetAllEvents");
            return JsonConvert.DeserializeObject<List<TimelineEvent>>(json);
        }

        /// <summary>
        /// Updates the counters for the file and image attachments
        /// </summary>
        public void UpdateAttachmentCounts()
        {
            AttachmentFilesCount = FileAttachments.Count();
            AttachmentImagesCount = ImageAttachments.Count();
        }
    }
}
