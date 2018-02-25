﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
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
            if (Path.GetFileName(filename) != Id)
            {
                throw new TimelineException("'Filename' should match attachment ID");
            }

            string url = await GenerateUploadPresignedUrlAsync(api);

            await api.UploadFileAsync(url, filename);
        }

        public async Task<string> DownloadAsync(ITimelineService api, string directory)
        {
            string url = await GenerateGetPresignedUrlAsync(api);
            string file = Path.Combine(directory, Id);

            await api.DownloadFileAsync(url, file);

            return file;
        }
    }
}
