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
    public class Timeline
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
        public int EventsCount { get; set; }

        public virtual Collection<TimelineEvent> TimelineEvents { get; set; }

        public static async Task<Timeline> CreateAsync(ITimelineService api, string title)
        {
            string json = await api.PutJsonAsync("Timeline/Create", new
            {
                TimelineId = Guid.NewGuid().ToString(),
                Title = title
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/EditTitle", new
            {
                TimelineId = Id,
                Title
            });
        }

        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/Delete", new
            {
                TimelineId = Id,
            });
        }

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

        public static async Task<Timeline> GetTimelineAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimeline", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        public static async Task<IList<Timeline>> GetTimelinesAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimelines");
            return JsonConvert.DeserializeObject<List<Timeline>>(json);
        }

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

        // Needed to deserialize the timeline list.
        private class TimelineCollection
        {
            public List<Timeline> Timelines { get; set; }
        }
    }
}
