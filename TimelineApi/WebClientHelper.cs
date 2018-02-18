﻿using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class WebClientHelper : IWebClientHelper
    {
        public Task<string> DownloadStringAsync(string url, NameValueCollection headers)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(headers);
                return client.DownloadStringTaskAsync(url);
            }
        }

        public Task<string> UploadStringAsync(string url, string body)
        {
            using (WebClient client = new WebClient())
            {
                return client.UploadStringTaskAsync(url, "PUT", body);
            }
        }

        public HttpStatusCode GetStatusCode(WebResponse response)
        {
            if (response is HttpWebResponse http)
            {
                return http.StatusCode;
            }
            return HttpStatusCode.OK;
        }

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
