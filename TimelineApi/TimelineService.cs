using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class TimelineService : ITimelineService
    {
        private readonly IWebClientHelper _client;
        private readonly string _baseUrl;
        private readonly string _authToken;
        private readonly string _tenantId;

        public TimelineService(string baseUrl, string authToken, string tenantId)
            : this(baseUrl, authToken, tenantId, new WebClientHelper()) { }

        public TimelineService(string baseUrl, string authToken, string tenantId, IWebClientHelper client)
        {
            _client = client;
            _baseUrl = baseUrl;
            _authToken = authToken;
            _tenantId = tenantId;
        }

        private string GetUrl(string resource)
        {
            return $"{_baseUrl}{resource}";
        }

        private static string CleanupResponse(string value)
        {
            // Need to remove these characters from response or JSON does not deserialize correctly. Not sure if this 
            // is an issue with IdeaGen's API, or something on our end. Anyway, this works.
            return value.Replace("\\", string.Empty).Trim('"');
        }

        private void HandleError(WebException ex)
        {
            // Check if this was a 400 or 500 message.
            var status = _client.GetStatusCode(ex.Response);
            if (status == HttpStatusCode.BadRequest || status == HttpStatusCode.InternalServerError)
            {
                // Get response message and throw new exception.
                string message = _client.GetResponseMessage(ex.Response);
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
                string response = await _client.UploadStringAsync(url, body.ToString());
                return CleanupResponse(response);
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
                string response = await _client.DownloadStringAsync(url, headers);
                return CleanupResponse(response);
            }
            catch (WebException ex)
            {
                HandleError(ex);

                throw; // Throw original exception if we don't handle it.
            }
        }
    }
}
