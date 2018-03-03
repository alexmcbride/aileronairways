using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class TimelineService : ITimelineService
    {
        private readonly IWebClientHelper _helper;
        private readonly string _baseUrl;
        private readonly string _authToken;
        private readonly string _tenantId;

        public TimelineService(string baseUrl, string authToken, string tenantId)
            : this(baseUrl, authToken, tenantId, new WebClientHelper()) { }

        public TimelineService(string baseUrl, string authToken, string tenantId, IWebClientHelper helper)
        {
            _helper = helper;
            _baseUrl = baseUrl;
            _authToken = authToken;
            _tenantId = tenantId;
        }

        private string GetUrl(string resource)
        {
            return $"{_baseUrl}{resource}";
        }

        private void HandleError(WebException ex)
        {
            // Check if this was a 400 or 500 message.
            var status = _helper.GetStatusCode(ex.Response);
            if (status == HttpStatusCode.BadRequest || status == HttpStatusCode.InternalServerError)
            {
                // Get response message and throw new exception.
                string message = _helper.GetResponseMessage(ex.Response);
                throw new TimelineException(message, ex);
            }
        }

        public async Task<string> PutJsonAsync(string resource, object request)
        {
            // Turn request into JSON and add auth stuff.
            JObject body = JObject.FromObject(request);
            body.Add("AuthToken", _authToken);
            body.Add("TenantId", _tenantId);

            try
            {
                // Make request and get JSON response.
                string url = GetUrl(resource);
                return await _helper.UploadStringAsync(url, body.ToString());
            }
            catch (WebException ex)
            {
                HandleError(ex);

                throw; // Throw original exception if we don't handle it.
            }
        }

        public Task<string> GetJsonAsync(string resource)
        {
            return GetJsonAsync(resource, new NameValueCollection());
        }

        public async Task<string> GetJsonAsync(string resource, NameValueCollection headers)
        {
            // Add auth stuff to headers.
            headers.Add("AuthToken", _authToken);
            headers.Add("TenantId", _tenantId);

            try
            {
                // Get JSON response.
                string url = GetUrl(resource);
                return await _helper.DownloadStringAsync(url, headers);
            }
            catch (WebException ex)
            {
                HandleError(ex);

                throw; // Throw original exception if we don't handle it.
            }
        }

        public Task UploadFileAsync(string url, string filename)
        {
            return _helper.UploadFileAsync(url, filename);
        }

        public Task DownloadFileAsync(string url, string filename)
        {
            return _helper.DownloadFileAsync(url, filename);
        }

        public void RenameFile(string oldName, string newName)
        {
            File.Copy(oldName, newName, overwrite: true);
            File.Delete(oldName);
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public void FileDelete(string filename)
        {
            File.Delete(filename);
        }
    }
}
