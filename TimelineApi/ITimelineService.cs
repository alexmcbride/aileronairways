using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public interface ITimelineService
    {
        Task<string> GetJsonAsync(string resource);
        Task<string> GetJsonAsync(string resource, NameValueCollection headers);
        Task<string> PutJsonAsync(string resource, object request);
    }
}