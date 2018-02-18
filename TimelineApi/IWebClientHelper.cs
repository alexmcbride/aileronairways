using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    /// <summary>
    /// Interface for WebClientHelper to make it mockable.
    /// </summary>
    public interface IWebClientHelper
    {        
        /// <summary>
        /// Downloads a string from the WebClient.
        /// </summary>
        /// <param name="url">The URL to download the string from.</param>
        /// <param name="headers">A collection of headers to add to the request.</param>
        /// <returns>The resulting string.</returns>
        Task<string> UploadStringAsync(string url, string body);

        /// <summary>
        /// Uploads a string using the WebClient and returns the response.
        /// </summary>
        /// <param name="url">The URL to upload the string to,</param>
        /// <param name="body">The body of the request as JSON</param>
        /// <returns>The response as a string.</returns>
        Task<string> DownloadStringAsync(string url, NameValueCollection headers);

        /// <summary>
        /// Gets an HttpStatusCode from a WebResponse.
        /// </summary>
        /// <param name="response">The response to get the code from.</param>
        /// <returns>The resulting code</returns>
        HttpStatusCode GetStatusCode(WebResponse response);

        /// <summary>
        /// Gets the message from a web response.
        /// </summary>
        /// <param name="response">The response to get the message from.</param>
        /// <returns>The message.</returns>
        string GetResponseMessage(WebResponse response);
    }
}