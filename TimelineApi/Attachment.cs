using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class Attachment : ModelBase
    {
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }

        public static async Task<Attachment> CreateAsync(ITimelineService api, string timelineEventId, string title)
        {
            string json = await api.PutJsonAsync("TimelineEventAttachment/Create", new
            {
                AttachmentId = Guid.NewGuid().ToString(),
                TimelineEventId = timelineEventId,
                Title = title
            });
            return JsonConvert.DeserializeObject<Attachment>(json);
        }
    }
}
