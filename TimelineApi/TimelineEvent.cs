using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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

        public string Location { get; set; }

        public async Task<TimelineEvent> CreateAsync(ITimelineService api)
        {
            string json = await api.PutJsonAsync("TimelineEvent/Create", new
            {
                TimelineEventId = Guid.NewGuid().ToString(),
                Title,
                Description,
                EventDateTime,
                Location,
                IsDeleted
            });
            var result = JsonConvert.DeserializeObject<TimelineEvent>(json);
            TenantId = result.TenantId;
            return this;
        }
    }
}
