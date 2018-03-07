using AileronAirwaysWeb.Services;
using Echelon.TimelineApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// A version of Timeline that uses in memory-caching.
    /// </summary>
    public class CachedTimeline : Timeline
    {
        private const string TimelineIdsKey = "timelineIds";

        private static void CacheTimeline(ICachedTimelineService api, Timeline timeline)
        {
            api.Set(timeline.Id, timeline);
            CacheTimelineId(api, timeline.Id);
        }

        private static void RemoveTimeline(ICachedTimelineService api, Timeline timeline)
        {
            api.Remove(timeline.Id);
            RemoveTimelineId(api, timeline.Id);
        }

        private static void CacheTimelineId(ICachedTimelineService api, string id)
        {
            if (api.TryGet(TimelineIdsKey, out List<string> timelineIds) && !timelineIds.Contains(id))
            {
                timelineIds.Add(id);
            }
            else
            {
                api.Set(TimelineIdsKey, new List<string> { id });
            }
        }

        private static void RemoveTimelineId(ICachedTimelineService api, string id)
        {
            if (api.TryGet(TimelineIdsKey, out List<string> timelineIds))
            {
                timelineIds.Remove(id);
            }
        }

        public async Task CacheEditAsync(ICachedTimelineService api)
        {
            await EditTitleAsync(api);
            CacheTimeline(api, this);
        }

        public async Task CacheDeleteAsync(ICachedTimelineService api)
        {
            await DeleteAsync(api);
            RemoveTimeline(api, this);
        }

        public static async Task<Timeline> CacheCreateAsync(ICachedTimelineService api, string title)
        { 
            var timeline = await CreateAsync(api, title);
            CacheTimeline(api, timeline);
            return timeline;
        }

        public static async Task<Timeline> CacheGetTimelineAsync(ICachedTimelineService api, string timelineId)
        {
            if (!api.TryGet(timelineId, out Timeline timeline))
            {
                timeline = await GetTimelineAsync(api, timelineId);
                CacheTimeline(api, timeline);
            }
            return timeline;
        }

        public static async Task<IList<Timeline>> CacheGetTimelinesAsync(ICachedTimelineService api)
        {
            IList<Timeline> timelines = new List<Timeline>();
            if (api.TryGet(TimelineIdsKey, out List<string> timelineIds))
            {
                foreach (var timelineId in timelineIds)
                {
                    if (api.TryGet(timelineId, out Timeline timeline))
                    {
                        timelines.Add(timeline);
                    }
                }
            }
            else
            {
                timelines = await GetTimelinesAsync(api);
                foreach (var timeline in timelines)
                {
                    CacheTimeline(api, timeline);
                }
            }
            return timelines;
        }
    }
}
