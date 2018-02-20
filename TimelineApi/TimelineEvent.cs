﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi
{
    public class TimelineEvent
    {   
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime EventDateTime { get; set; }

        public string Location { get; set; }

        public static Task<TimelineEvent> CreateAsync(ITimelineService api, string title, DateTime eventDateTime)
        {
            return CreateAsync(api, title, string.Empty, eventDateTime, string.Empty);
        }

        public static async Task<TimelineEvent> CreateAsync(ITimelineService api, string title, string description, DateTime eventDateTime, string location)
        {
            string json = await api.PutJsonAsync("TimelineEvent/Create", new
            {
                TimelineEventId = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
                EventDateTime = eventDateTime.Ticks.ToString(),
                Location = location
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }
        //Mie added this to generate id from controller to save data, if no id on controller it did not save the link to database
        public static async Task<TimelineEvent> CreateAsync(ITimelineService api, string id,string title, string description, DateTime eventDateTime, string location)
        {
            string json = await api.PutJsonAsync("TimelineEvent/Create", new
            {
                TimelineEventId = id,
                Title = title,
                Description = description,
                EventDateTime = eventDateTime.Ticks.ToString(),
                Location = location
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }

        public Task EditTitleAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditTitle", new
            {
                TimelineEventId = Id,
                Title
            });
        }

        public Task EditDescriptionAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditDescription", new
            {
                TimelineEventId = Id,
                Description
            });
        }

        public Task EditLocationAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditLocation", new
            {
                TimelineEventId = Id,
                Location
            });
        }

        public Task EditEventDateTimeAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/EditEventDateTime", new
            {
                TimelineEventId = Id,
                EventDateTime = EventDateTime.Ticks.ToString()
            });
        }

        public Task DeleteAsync(ITimelineService api)
        {
            return api.PutJsonAsync("TimelineEvent/Delete", new
            {
                TimelineEventId = Id
            });
        }

        public static Task<TimelineEvent> GetTimelineEventAsync(ITimelineService api, LinkedEvent linkedEvent)
        {
            return GetTimelineEventAsync(api, linkedEvent.TimelineEventId);
        }

        public static async Task<TimelineEvent> GetTimelineEventAsync(ITimelineService api, string timelineEventId)
        {
            string json = await api.GetJsonAsync("TimelineEvent/GetTimelineEvent", new NameValueCollection
            {
                { "TimelineEventId", timelineEventId }
            });
            return JsonConvert.DeserializeObject<TimelineEvent>(json);
        }

        public static async Task<IList<LinkedEvent>> GetLinkedEventsAsync(ITimelineService api, string timelineId)
        {
            string json = await api.GetJsonAsync("Timeline/GetEvents", new NameValueCollection
            {
                { "TimelineId", timelineId }
            });
            return JsonConvert.DeserializeObject<List<LinkedEvent>>(json);
        }
    }
}
