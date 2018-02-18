﻿using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public interface IWebClientHelper
    {        
        Task<string> UploadStringAsync(string url, string body);
        Task<string> DownloadStringAsync(string url, NameValueCollection headers);
        HttpStatusCode GetStatusCode(WebResponse response);
        string GetResponseMessage(WebResponse response);
    }
}