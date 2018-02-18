using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    /// <summary>
    /// Class to represent an API timeline.
    /// </summary>
    public class Timeline
    {
        /// <summary>
        /// Gets or sets the Id of the timeline.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the timeline.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the timeline.
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CreationTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the a property indicating if this timeline has been deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tenant who created the timeline.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Creates a new timeline on the API and returns the result.
        /// </summary>
        /// <param name="api">The API to create the timeline on.</param>
        /// <param name="title">The title for the new timeline</param>
        /// <returns>A populated timeline object.</returns>
        public static async Task<Timeline> CreateAsync(ITimelineService api, string title)
        {
            string json = await api.PutJsonAsync("Timeline/Create", new
            {
                TimelineId = Guid.NewGuid().ToString(),
                Title = title
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        /// <summary>
        /// Saves an edited title to the API.
        /// </summary>
        /// <param name="api">The API to save the edited title to.</param>
        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/EditTitle", new
            {
                TimelineId = Id,
                Title
            });
        }

        /// <summary>
        /// Deletes the timeline.
        /// </summary>
        /// <param name="api">The API to delete the timeline from.</param>
        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("Timeline/Delete", new
            {
                TimelineId = Id,
            });
        }

        /// <summary>
        /// Gets a timeline from the API.
        /// </summary>
        /// <param name="api">The API to fetch the timeline from.</param>
        /// <param name="timelineId">The ID of the timeline to fetch.</param>
        /// <returns>The timeline object.</returns>
        public static async Task<Timeline> GetTimelineAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimeline", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<Timeline>(json);
        }

        /// <summary>
        /// Gets all timelines from the API.
        /// </summary>
        /// <param name="api">The API to fetch timelines from.</param>
        /// <returns>A list of timeline objects.</returns>
        public static async Task<IList<Timeline>> GetTimelinesAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("Timeline/GetTimelines");
            return JsonConvert.DeserializeObject<List<Timeline>>(json);
        }

        public Task LinkEventAsync(ITimelineService api, TimelineEvent evt)
        {
            return api.PutJsonAsync("Timeline/LinkEvent", new
            {
                TimelineId = Id,
                EventId = evt.Id
            });
        }

        public Task UnlinkEventAsync(ITimelineService api, TimelineEvent evt)
        {
            return api.PutJsonAsync("Timeline/UnlinkEvent", new
            {
                TimelineId = Id,
                EventId = evt.Id
            });
        }

        public async Task<IList<LinkedEvent>> GetEventsAsync(ITimelineService api)
        {
            string json = await api.GetJsonAsync("Timeline/GetEvents", new NameValueCollection
            {
                { "TimelineId", Id }
            });
            return JsonConvert.DeserializeObject<List<LinkedEvent>>(json);
        }
    }
}
