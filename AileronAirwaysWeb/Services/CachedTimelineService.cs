using Echelon.TimelineApi;
using Microsoft.Extensions.Caching.Memory;

namespace AileronAirwaysWeb.Services
{
    public class CachedTimelineService : TimelineService, ICachedTimelineService
    {
        private readonly IMemoryCache _cache;

        public CachedTimelineService(string baseUrl, string authToken, string tenantId, string rootFolder, IMemoryCache cache)
            : this(baseUrl, authToken, tenantId, rootFolder, new WebClientHelper(), cache)
        {
            // Empty
        }

        public CachedTimelineService(string baseUrl, string authToken, string tenantId, string rootFolder, IWebClientHelper helper, IMemoryCache cache)
        : base(baseUrl, authToken, tenantId, rootFolder, helper)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }
    }
}
