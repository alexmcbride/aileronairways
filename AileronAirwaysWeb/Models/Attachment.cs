using AileronAirwaysWeb.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// Class to represent an attachment.
    /// </summary>
    public class Attachment
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; }

        // Foreign key
        public string TimelineEventId { get; set; }

        [JsonIgnore]
        public virtual TimelineEvent TimelineEvent { get; set; }

        /// <summary>
        /// Gets the name of the attachment in the cache, which is the ID and the file extension.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return $"{Id}{Path.GetExtension(Title)}"; }
        }

        /// <summary>
        /// Gets the location of the attachment in the cache in the web server.
        /// </summary>
        [JsonIgnore]
        public string FileName
        {
            get { return $"~/cache/{Name}"; }
        }

        /// <summary>
        /// Gets the content type of the attachment.
        /// </summary>
        [JsonIgnore]
        public string ContentType
        {
            get
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types
                var ext = Path.GetExtension(Title).ToLower();
                if (ext == ".png")
                {
                    return "image/png";
                }
                else if (ext == ".jpg" || ext == ".jpeg")
                {
                    return "image/jpeg";
                }
                else if (ext == ".gif")
                {
                    return "image/gif";
                }
                return "application/octet-stream"; // General file content type.
            }
        }

        /// <summary>
        /// Gets if the attachment is an image or not.
        /// </summary>
        public bool IsImage
        {
            get { return ContentType != "application/octet-stream"; }
        }

        /// <summary>
        /// Creates a new attachment on the API for the specified timeline ID.
        /// </summary>
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

        /// <summary>
        /// Edits an attchment title on the API.
        /// </summary>
        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEventAttachment/EditTitle", new
            {
                AttachmentId = Id,
                Title
            });
        }

        /// <summary>
        /// Deletes the attachment from the API.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task DeleteAsync(ITimelineService api)
        {
            await api.PutJsonAsync("TimelineEventAttachment/Delete", new
            {
                AttachmentId = Id
            });

            // Delete attachment from disk if it's there.
            var file = Path.Combine(api.CacheFolder, Name);
            if (api.FileExists(file))
            {
                api.FileDelete(file);
            }
        }

        /// <summary>
        /// Get a URL from the API that can be used to upload an attachment.
        /// </summary>
        public Task<string> GenerateUploadPresignedUrlAsync(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateUploadPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        /// <summary>
        /// Get a URL from the API that can be used to download an attachment.
        /// </summary>
        public Task<string> GenerateGetPresignedUrlAsync(ITimelineService api)
        {
            return api.GetJsonAsync("TimelineEventAttachment/GenerateGetPresignedUrl", new NameValueCollection
            {
                { "AttachmentId", Id }
            });
        }

        /// <summary>
        /// Gets the API attachment with the specified ID.
        /// </summary>
        public static async Task<Attachment> GetAttachmentAsync(ITimelineService api, string attachmentId)
        {
            string json = await api.GetJsonAsync("TimelineEventAttachment/GetAttachment", new NameValueCollection
            {
                { "AttachmentId", attachmentId }
            });
            return JsonConvert.DeserializeObject<Attachment>(json);
        }

        /// <summary>
        /// Gets all attachments for the timeline with the specified ID.
        /// </summary>
        public static async Task<IList<Attachment>> GetAttachmentsAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEventAttachment/GetAttachments", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<List<Attachment>>(json);
        }

        /// <summary>
        /// Uploads the attachment with the specified filename to the API using this attachment object.
        /// </summary>
        public async Task UploadAsync(ITimelineService api, string filename)
        {
            string url = await GenerateUploadPresignedUrlAsync(api);

            await api.UploadFileAsync(url, filename);
        }

        /// <summary>
        /// Downloads the attachment file, or gets it from the cache if it's already been downloaded.
        /// </summary>
        public async Task<string> DownloadOrCacheAsync(ITimelineService api)
        {
            string url = await GenerateGetPresignedUrlAsync(api);

            // Download attachment if it doesn't exist in the cache.
            var file = Path.Combine(api.CacheFolder, Name);
            if (!api.FileExists(file))
            {
                await api.DownloadFileAsync(url, file);
            }

            Debug.WriteLine("URL: " + url);
            Debug.WriteLine("Filename: " + file);

            return file;
        }

        /// <summary>
        /// Creates a new attachment and uploads the file in a single action, attaching it to the specified event.
        /// </summary>
        /// <param name="api">The API to create the attachment on.</param>
        /// <param name="eventId">The ID of the event the attachment is being attached to.</param>
        /// <param name="filename">The name of the attachment file on the local web server.</param>
        /// <param name="fileStream">A stream object containing the file contents.</param>
        /// <returns>The new attachment</returns>
        public static async Task<Attachment> CreateAndUploadAsync(ITimelineService api, string eventId, string filename, Stream fileStream)
        {
            var attachment = await CreateAsync(api, eventId, filename);

            string temp = Path.GetTempFileName();
            try
            {
                await CopyFileFromStream(api, fileStream, temp);
                await attachment.UploadAsync(api, temp);
            }
            finally
            {
                if (api.FileExists(temp))
                {
                    api.FileDelete(temp);
                }
            }

            return attachment;
        }

        // Copies a file from the stream into the specified temp file.
        private static async Task CopyFileFromStream(ITimelineService api, Stream stream, string tempFile)
        {
            Stream tempStream = null;
            try
            {
                tempStream = api.FileOpenWrite(tempFile);
                await stream.CopyToAsync(tempStream);
            }
            finally
            {
                // We do it like this to prevent "object disposed" errors in tests.
                if (tempStream != null)
                {
                    api.DisposeStream(tempStream);
                }
            }
        }
    }
}
