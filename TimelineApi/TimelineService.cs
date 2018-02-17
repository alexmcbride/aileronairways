using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace Echelon.TimelineApi
{
    /// <summary>
    /// Service to aid with interacting with the API.
    /// </summary>
    public class TimelineService : ITimelineService
    {
        private readonly IWebClientHelper _client;
        private readonly string _baseUrl;
        private readonly string _authToken;
        private readonly string _tenantId;

        /// <summary>
        /// Creates a new TimelineService object.
        /// </summary>
        /// <param name="baseUrl">The base URL of the IdeaGen server</param>
        /// <param name="authToken">The authentication token needed by the API</param>
        /// <param name="tenantId">The ID which identifies our team to IdeaGen</param>
        public TimelineService(string baseUrl, string authToken, string tenantId)
            : this(new WebClientHelper(), baseUrl, authToken, tenantId) { }

        /// <summary>
        /// Creates a new TimelineService object.
        /// </summary>
        /// <param name="client">The web client helper used for web requests</param>
        /// <param name="baseUrl">The base URL of the IdeaGen server</param>
        /// <param name="authToken">The authentication token needed by the API</param>
        /// <param name="tenantId">The ID which identifies our team to IdeaGen</param>
        public TimelineService(IWebClientHelper client, string baseUrl, string authToken, string tenantId)
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

        /// <summary>
        /// Makes a HTTP PUT request to the API and returns the response as JSON.
        /// </summary>
        /// <param name="resource">The resource to use .e.g. Timeline/Create</param>
        /// <param name="request">An object contaning the body parameters to use for the request</param>
        /// <returns>The response as JSON.</returns>
        public string PutJson(string resource, object request)
        {
            // Turn request into JSON and add auth stuff.
            JObject body = JObject.FromObject(request);
            body.Add("AuthToken", _authToken);
            body.Add("TenantId", _tenantId);

            // Make request and get JSON response.
            string url = GetUrl(resource);
            string json = _client.UploadString(url, body.ToString());
            return JsonConvert.DeserializeObject(json).ToString();
        }

        public string GetJson(string resource)
        {
            return GetJson(resource, new NameValueCollection());
        }

        /// <summary>
        /// Makes a HTTP GET request to the API and returns the response as JSON.
        /// </summary>
        /// <param name="resource">The resource to use .e.g. Timeline/GetTimelines</param>
        /// <param name="headers">A collection of header parameters to add to the request.</param>
        /// <returns>The response as JSON.</returns>
        public string GetJson(string resource, NameValueCollection headers)
        {
            // Add auth stuff to headers.
            headers.Add("AuthToken", _authToken);
            headers.Add("TenantId", _tenantId);

            // Get JSON response.
            string url = GetUrl(resource);
            string json = _client.DownloadString($"{_baseUrl}{resource}", headers);
            return JsonConvert.DeserializeObject(json).ToString();
        }
    }
}
