using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class Timeline : ModelBase
    {
        public string Title { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreationTimeStamp { get; set; }
        public bool IsDeleted { get; set; }

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

        public static async Task<List<TimelineWithEvents>> GetAllTimelinesAndEventsAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("General/GetAllTimelinesAndEvent");
            var timelines = JsonConvert.DeserializeObject<TimelineCollection>(json);
            if (timelines != null)
            {
                return timelines.Timelines;
            }
            return null;
        }

        private class TimelineCollection
        {
            public List<TimelineWithEvents> Timelines { get; set; }
        }
    }
}
