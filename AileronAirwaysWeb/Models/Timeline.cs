using AileronAirwaysWeb.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// Class to represent a Timeline.
    /// </summary>
    public class Timeline
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Counter to hold number of events
        /// </summary>
        public int EventsCount { get; set; }

        /// <summary>
        /// The events associated with this timeline.
        /// </summary>
        public virtual Collection<TimelineEvent> TimelineEvents { get; set; }

        /// <summary>
        /// Creates a new timeline with the title.
        /// </summary>
        public static async Task<Timeline> CreateAsync(ITimelineService api, string title)
        {
            string json = await api.PutJsonAsync("Timeline/Create", new
            {
                TimelineId = Guid.NewGuid().ToString(),
                Title = title
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        /// <summary>
        /// Edits the title of this timeline, saving it to the API.
        /// </summary>
        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/EditTitle", new
            {
                TimelineId = Id,
                Title
            });
        }

        /// <summary>
        /// Deletes this timeline from the API.
        /// </summary>
        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/Delete", new
            {
                TimelineId = Id,
            });
        }

        /// <summary>
        /// Updates calculated columns, which are for columns in the database which are not saved in the 
        /// API but need to be recalculated whenever it is downloaded.
        /// </summary>
        public void UpdateCalculatedColumns()
        {
            foreach (var @event in TimelineEvents)
            {
                @event.UpdateAttachmentCounts();
            }

            if (TimelineEvents != null)
            {
                EventsCount = TimelineEvents.Count();
            }
        }

        /// <summary>
        /// Gets the timeline with this ID from the API.
        /// </summary>
        public static async Task<Timeline> GetTimelineAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimeline", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        /// <summary>
        /// Gets all timelines from the API.
        /// </summary>
        public static async Task<IList<Timeline>> GetTimelinesAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimelines");
            return JsonConvert.DeserializeObject<List<Timeline>>(json);
        }

        /// <summary>
        /// Gets all timelines, events, and attachments from the API in one go.
        /// </summary>
        public static async Task<List<Timeline>> GetAllTimelinesAndEventsAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("Timeline/GetAllTimelinesAndEvent");
            var timelines = JsonConvert.DeserializeObject<TimelineCollection>(json);
            if (timelines != null)
            {
                foreach (var timeline in timelines.Timelines)
                {
                    timeline.UpdateCalculatedColumns();
                }
                return timelines.Timelines;
            }
            return null;
        }

        /// <summary>
        /// A synchronous version of GetAllTimelinesAndEventsAsync() that will block the calling thread.
        /// </summary>
        public static List<Timeline> GetAllTimelinesAndEvents(ITimelineService api)
        {
            var task = api.GetJsonAsync("Timeline/GetAllTimelinesAndEvent");
            task.Wait();
            string json = task.Result;
            var timelines = JsonConvert.DeserializeObject<TimelineCollection>(json);
            if (timelines != null)
            {
                foreach (var timeline in timelines.Timelines)
                {
                    timeline.UpdateCalculatedColumns();
                }
                return timelines.Timelines;
            }
            return null;
        }

        // Wee temp class needed to deserialize the timeline list.
        private class TimelineCollection
        {
            public List<Timeline> Timelines { get; set; }
        }
    }
}
