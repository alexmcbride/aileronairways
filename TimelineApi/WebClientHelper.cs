using System.Collections.Specialized;
using System.Net;

namespace Echelon.TimelineApi
{
    /// <summary>
    /// Helper thay wraps a WebClient, in order to make testing easier.
    /// </summary>
    public class WebClientHelper : IWebClientHelper
    {
        /// <summary>
        /// Downloads a string from the WebClient.
        /// </summary>
        /// <param name="url">The URL to download the string from.</param>
        /// <param name="headers">A collection of headers to add to the request.</param>
        /// <returns>The resulting string.</returns>
        public string DownloadString(string url, NameValueCollection headers)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(headers);
                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Uploads a string using the WebClient and returns the response.
        /// </summary>
        /// <param name="url">The URL to upload the string to,</param>
        /// <param name="body">The body of the request as JSON</param>
        /// <returns>The response as a string.</returns>
        public string UploadString(string url, string body)
        {
            using (WebClient client = new WebClient())
            {
                return client.UploadString(url, "PUT", body);
            }
        }
    }
}
