using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Services
{
    /// <summary>
    /// Wrapper for WebClient to make it testable.
    /// </summary>
    public class WebClientHelper : IWebClientHelper
    {
        /// <summary>
        /// Downloads a string from the webclient, while letting you set some header stuff.
        /// </summary>
        public Task<string> DownloadStringAsync(string url, NameValueCollection headers)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(headers);
                return client.DownloadStringTaskAsync(url);
            }
        }

        /// <summary>
        /// Uploads a string and lets you set the request body.
        /// </summary>
        public Task<string> UploadStringAsync(string url, string body)
        {
            using (WebClient client = new WebClient())
            {
                return client.UploadStringTaskAsync(url, "PUT", body);
            }
        }

        /// <summary>
        /// Gets a status code from a WebResponse.
        /// </summary>
        public HttpStatusCode GetStatusCode(WebResponse response)
        {
            if (response is HttpWebResponse http)
            {
                return http.StatusCode;
            }
            return 0;
        }

        /// <summary>
        /// Gets a message from a WebResponse.
        /// </summary>
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

        /// <summary>
        /// Downloads a file to disk
        /// </summary>
        public Task DownloadFileAsync(string url, string filename)
        {
            using (var client = new WebClient())
            {
                return client.DownloadFileTaskAsync(url, filename);
            }
        }

        /// <summary>
        /// Uploads a file from disk.
        /// </summary>
        public Task UploadFileAsync(string url, string filename)
        {
            using (var client = new WebClient())
            {
                return client.UploadFileTaskAsync(url, "PUT", filename);
            }
        }
    }
}
