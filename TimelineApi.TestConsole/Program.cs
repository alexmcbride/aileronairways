using System;
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

            string id = "255c6ab0-79bc-4d2d-8793-bd508c7c39f9";

            Console.WriteLine("Waiting on GetTimeline");
            Timeline timeline = await Timeline.GetTimelineAsync(api, id);
            Console.WriteLine("Done");

            TimelineEvent evt = new TimelineEvent();
            evt.Title = "Test Event Title 3";
            evt.Description = "Description...";
            evt.EventDateTime = DateTime.Now;
            evt.Location = "-1.1234,1.1234";

            Console.WriteLine("Waiting on CreateEvent");
            await evt.CreateAsync(api);
            Console.WriteLine("Done");

            Console.WriteLine("Waiting on LinkEvent");
            await timeline.LinkEventAsync(api, evt);
            Console.WriteLine("Done");

            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Waiting for linked events");
            IList<LinkedEvent> linkedEvents = await TimelineEvent.GetLinkedEventsAsync(api, id);
            Console.WriteLine("Done ({0})", linkedEvents.Count);

            IEnumerable<Task<TimelineEvent>> eventTasks = linkedEvents.Select(l => TimelineEvent.GetTimelineEventAsync(api, l));

            // Wait for all events to download.
            Console.WriteLine("Waiting for events");
            var timelineEvents = await Task.WhenAll(eventTasks);

            sw.Stop();

            foreach (var timelineEvent in timelineEvents)
            {
                DisplayTimelineEvent(timelineEvent);
            }

            Console.WriteLine("Done");
            Console.WriteLine("Elapsed: {0} ms", sw.ElapsedMilliseconds);
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
        }

        private static void DisplayTimeline(Timeline timeline)
        {
            Console.WriteLine("Timeline:");
            Console.WriteLine($"ID: {timeline.Id}");
            Console.WriteLine($"Title: {timeline.Title}");
            Console.WriteLine($"Creation: {timeline.CreationTimeStamp}");
            Console.WriteLine($"Is Deleted: {timeline.IsDeleted}");
            Console.WriteLine($"Tenant ID: {timeline.TenantId}");
            Console.Write("----");
            Console.WriteLine();
        }
    }
}
