using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    /// <summary>
    /// Helper that wraps a WebClient, in order to make testing easier.
    /// </summary>
    public class WebClientHelper : IWebClientHelper
    {
        /// <summary>
        /// Downloads a string from the WebClient.
        /// </summary>
        /// <param name="url">The URL to download the string from.</param>
        /// <param name="headers">A collection of headers to add to the request.</param>
        /// <returns>The resulting string.</returns>
        public Task<string> DownloadStringAsync(string url, NameValueCollection headers)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(headers);
                return client.DownloadStringTaskAsync(url);
            }
        }

        /// <summary>
        /// Uploads a string using the WebClient and returns the response.
        /// </summary>
        /// <param name="url">The URL to upload the string to,</param>
        /// <param name="body">The body of the request as JSON</param>
        /// <returns>The response as a string.</returns>
        public Task<string> UploadStringAsync(string url, string body)
        {
            using (WebClient client = new WebClient())
            {
                return client.UploadStringTaskAsync(url, "PUT", body);
            }
        }

        /// <summary>
        /// Gets an HttpStatusCode from a WebResponse.
        /// </summary>
        /// <param name="response">The response to get the code from.</param>
        /// <returns>The resulting code</returns>
        public HttpStatusCode GetStatusCode(WebResponse response)
        {
            if (response is HttpWebResponse http)
            {
                return http.StatusCode;
            }
            return HttpStatusCode.OK;
        }

        /// <summary>
        /// Gets the message from a web response.
        /// </summary>
        /// <param name="response">The response to get the message from.</param>
        /// <returns>The message.</returns>
        public string GetResponseMessage(WebResponse response)
        {
            if (response is HttpWebResponse http)
            {
                using (Stream stream = http.GetResponseStream())
                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
