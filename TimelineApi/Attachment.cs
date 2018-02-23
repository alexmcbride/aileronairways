using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEventAttachment/EditTitle", new
            {
                AttachmentId = Id,
                Title
            });
        }

        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEventAttachment/Delete", new
            {
                AttachmentId = Id
            });
        }

        public Task<string> GenerateUploadPresignedUrl(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateUploadPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        public Task<string> GenerateGetPresignedUrl(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateGetPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        public static async Task<Attachment> GetAttachment(ITimelineService api, string attachmentId)
        {
            string json = await api.GetJsonAsync("TimelineEventAttachment/GetAttachment", new NameValueCollection
            {
                { "AttachmentId", attachmentId }
            });
            return JsonConvert.DeserializeObject<Attachment>(json);
        }
    }
}
