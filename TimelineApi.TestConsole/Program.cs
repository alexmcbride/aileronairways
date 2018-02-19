﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.TestConsole
{
    class Program
    {
        const string BaseUrl = "https://gcu.ideagen-development.com/";
        const string AuthToken = "5e906627-a997-4ab1-94c5-07fcb6c9383d";
        const string TenantId = "Team3";

        static void Main(string[] args)
        {
            RunTestsAsync();

            // Stop program from exiting.
            Console.ReadKey(true);
        }

        private static async void RunTestsAsync()
        {
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            // Get timeline with this ID.
            Timeline timeline = await Timeline.GetTimelineAsync(api, "255c6ab0-79bc-4d2d-8793-bd508c7c39f9");
            DisplayTimeline(timeline);

            // Create new event.
            TimelineEvent evt = await TimelineEvent.CreateAsync(api, "Another New Event", "Event description", DateTime.Now, "-1.1234,1.1234");
            DisplayTimelineEvent(evt);

            // Link timeline and event together.
            await timeline.LinkEventAsync(api, evt);

            // Get list of linked events.
            IList<LinkedEvent> linkedEvents = await timeline.GetEventsAsync(api);

            // Loop through each linked events.
            foreach (var linkedEvent in linkedEvents)
            {
                // Get the event associated with this link event.
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(api, linkedEvent);
                DisplayTimelineEvent(timelineEvent);
            }
        }

        private static void DisplayTimelineEvent(TimelineEvent evt)
        {
            Console.WriteLine($"Id: {evt.Id}");
            Console.WriteLine($"Title: {evt.Title}");
            Console.WriteLine($"Description: {evt.Description}");
            Console.WriteLine($"EventDateTime: {evt.EventDateTime}");
            Console.WriteLine($"Location: {evt.Location}");
            Console.WriteLine($"IsDeleted: {evt.IsDeleted}");
            Console.WriteLine($"TenantId: {evt.TenantId}");
            Console.WriteLine();
        }

        private static void DisplayTimeline(Timeline timeline)
        {
            Console.WriteLine("Timeline:");
            Console.WriteLine($"ID: {timeline.Id}");
            Console.WriteLine($"Title: {timeline.Title}");
            Console.WriteLine($"Creation: {timeline.CreationTimeStamp}");
            Console.WriteLine($"Is Deleted: {timeline.IsDeleted}");
            Console.WriteLine($"Tenant ID: {timeline.TenantId}");
            Console.WriteLine();
        }
    }
}
