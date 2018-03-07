﻿using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Services
{
    public interface IWebClientHelper
    {        
        Task<string> UploadStringAsync(string url, string body);
        Task<string> DownloadStringAsync(string url, NameValueCollection headers);
        HttpStatusCode GetStatusCode(WebResponse response);
        string GetResponseMessage(WebResponse response);
        Task DownloadFileAsync(string url, string filename);
        Task UploadFileAsync(string url, string filename);
    }
}