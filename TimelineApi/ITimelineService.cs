using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public interface ITimelineService
    {
        Task<string> GetJsonAsync(string resource);
        Task<string> GetJsonAsync(string resource, NameValueCollection headers);
        Task<string> PutJsonAsync(string resource, object request);
        Task UploadFileAsync(string url, Stream fileStream);
        Task DownloadFileAsync(string url, string filename);
    }
}