using System.Collections.Specialized;

namespace Echelon.TimelineApi
{
    public interface ITimelineService
    {
        string GetJson(string resource);
        string GetJson(string resource, NameValueCollection headers);
        string PutJson(string resource, object request);
    }
}