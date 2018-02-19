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

            Timeline timeline = await Timeline.GetTimelineAsync(api, "255c6ab0-79bc-4d2d-8793-bd508c7c39f9");
            DisplayTimeline(timeline);

            TimelineEvent evt = new TimelineEvent();
            evt.Title = "New Event Title";
            evt.Description = "jcdhhd";
            evt.EventDateTime = DateTime.Now;
            evt.Location = "sdsds"; 
            await evt.CreateAsync(api);
            DisplayTimelineEvent(evt);

            await timeline.LinkEventAsync(api, evt);

            IList<LinkedEvent> linkedEvents = await timeline.GetEventsAsync(api);
            foreach (var item in linkedEvents)
            {
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(api, item);
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
