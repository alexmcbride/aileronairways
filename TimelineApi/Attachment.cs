using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class Attachment : ModelBase
    {
        public string Title { get; set; }
        public string TimelineEventId { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public string Name
        {
            get
            {
                return $"{Id}{Path.GetExtension(Title)}";
            }
        }

        [JsonIgnore]
        public string FileName
        {
            get { return $"~/cache/{Name}"; }
        }

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

        public async Task DeleteAsync(ITimelineService api, string cacheFolder)
        {
            await api.PutJsonAsync("TimelineEventAttachment/Delete", new
            {
                AttachmentId = Id
            });

            var file = Path.Combine(cacheFolder, Name);
            if (api.FileExists(file))
            {
                api.FileDelete(file);
            }
        }

        public Task<string> GenerateUploadPresignedUrlAsync(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateUploadPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        public Task<string> GenerateGetPresignedUrlAsync(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateGetPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        public static async Task<Attachment> GetAttachmentAsync(ITimelineService api, string attachmentId)
        {
            string json = await api.GetJsonAsync("TimelineEventAttachment/GetAttachment", new NameValueCollection
            {
                { "AttachmentId", attachmentId }
            });
            return JsonConvert.DeserializeObject<Attachment>(json);
        }

        public static async Task<IList<Attachment>> GetAttachmentsAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEventAttachment/GetAttachments", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<List<Attachment>>(json);
        }

        public async Task UploadAsync(ITimelineService api, string filename)
        {
            string url = await GenerateUploadPresignedUrlAsync(api);

            await api.UploadFileAsync(url, filename);
        }

        public async Task DownloadAsync(ITimelineService api, string cacheFolder)
        {
            string url = await GenerateGetPresignedUrlAsync(api);

            // Download attachment if it doesn't exist in the cache.
            var file = Path.Combine(cacheFolder, Name);
            if (!api.FileExists(file))
            {
                await api.DownloadFileAsync(url, file);
            }

            Debug.WriteLine("URL: " + url);
            Debug.WriteLine("Filename: " + file);
        }
    }
}
