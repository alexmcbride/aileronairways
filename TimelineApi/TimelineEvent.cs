using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class TimelineEvent : ModelBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EventDateTime { get; set; }
        public string Location { get; set; }

        public static Task<TimelineEvent> CreateAsync(ITimelineService api, string title, string description, DateTime eventDateTime, string location)
        {
            return CreateAsync(api, Guid.NewGuid().ToString(), title, description, eventDateTime, location);
        }

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

        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditTitle", new
            {
                TimelineEventId = Id,
                Title
            });
        }

        public Task EditDescriptionAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditDescription", new
            {
                TimelineEventId = Id,
                Description
            });
        }

        public Task EditLocationAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditLocation", new
            {
                TimelineEventId = Id,
                Location
            });
        }

        public Task EditEventDateTimeAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditEventDateTime", new
            {
                TimelineEventId = Id,
                EventDateTime = EventDateTime.Ticks.ToString()
            });
        }

        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/Delete", new
            {
                TimelineEventId = Id
            });
        }

        public static async Task<TimelineEvent> GetEventAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEvent/GetTimelineEvent", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }

        public static async Task<IList<LinkedEvent>> GetEventsAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetEvents", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<List<LinkedEvent>>(json);
        }

        public Task LinkEventAsync(ITimelineService api, string timelineId)
        {
            return api.PutJsonAsync("Timeline/LinkEvent", new
            {
                TimelineId = timelineId,
                EventId = Id
            });
        }

        public Task UnlinkEventAsync(ITimelineService api, string timelineId)
        {
            return api.PutJsonAsync("Timeline/UnlinkEvent", new
            {
                TimelineId = timelineId,
                EventId = Id
            });
        }

        public Task EditAsync(ITimelineService api)
        {
            // If you perform a create and keep the ID the same then it overwrites the exsting event.
            return CreateAsync(api, Id, Title, Description, EventDateTime, Location);
        }
    }
}
