using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class TimelineEvent
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EventDateTime { get; set; }

        public string Location
        {
            get { return $"{Longitude},{Latitude}"; }
            set
            {
                string[] tokens = value?.Split(',');
                if (tokens != null && tokens.Length == 2 && 
                    double.TryParse(tokens[0], out double lon) && 
                    double.TryParse(tokens[1], out double lat))
                {
                    Longitude = lon;
                    Latitude = lat;
                }
            }
        }

        [JsonIgnore]
        public double Longitude { get; set; }

        [JsonIgnore]
        public double Latitude { get; set; }

        public async Task<TimelineEvent> CreateAsync(ITimelineService api)
        {
            string json = await api.PutJsonAsync("TimelineEvent/Create", new
            {
                TimelineEventId = Guid.NewGuid().ToString(),
                Title,
                Description,
                EventDateTime,
                Location
            });
            var result = JsonConvert.DeserializeObject<TimelineEvent>(json);
            TenantId = result.TenantId;
            return result;
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
                EventDateTime
            });
        }

        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/Delete", new
            {
                TimelineEventId = Id
            });
        }

        public static Task<TimelineEvent> GetTimelineEventAsync(ITimelineService api, LinkedEvent linkedEvent)
        {
            return GetTimelineEventAsync(api, linkedEvent.TimelineEventId);
        }

        public static async Task<TimelineEvent> GetTimelineEventAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEvent/GetTimelineEvent", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }
    }
}
