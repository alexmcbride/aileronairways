using System.Collections.Specialized;

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
        string UploadString(string url, string body);

        /// <summary>
        /// Uploads a string using the WebClient and returns the response.
        /// </summary>
        /// <param name="url">The URL to upload the string to,</param>
        /// <param name="body">The body of the request as JSON</param>
        /// <returns>The response as a string.</returns>
        string DownloadString(string url, NameValueCollection headers);
    }
}