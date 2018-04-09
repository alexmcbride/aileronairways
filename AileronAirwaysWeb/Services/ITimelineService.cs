using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Services
{
    public interface ITimelineService
    {
        string CacheFolder { get; }
        Task<string> GetJsonAsync(string resource);
        Task<string> GetJsonAsync(string resource, NameValueCollection headers);
        Task<string> PutJsonAsync(string resource, object request);
        Task UploadFileAsync(string url, string filename);
        Task DownloadFileAsync(string url, string filename);
        bool FileExists(string filename);
        void FileDelete(string filename);
        Stream FileOpenWrite(string filename);
        void DisposeStream(Stream stream);
        Task<bool> IsOfflineAsync();
    }
}