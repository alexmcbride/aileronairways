using Echelon.TimelineApi;

namespace AileronAirwaysWeb.Services
{
    public interface ICachedTimelineService : ITimelineService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Remove(string key);
        bool TryGet<T>(string key, out T value);
    }

}
