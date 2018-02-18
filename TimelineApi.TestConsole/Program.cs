using System;
using System.Collections.Generic;
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
            // Create new API object and pass in our parameters.
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            // Test methods.
            RunTestsAsync(api);

            // Stop program from exiting.
            Console.ReadKey(true);
        }

        private static async void RunTestsAsync(ITimelineService api)
        {
            //await TestGetTimelinesAsync(api);
            //await TestTimelineActionsAsync(api);
            await TestCreateEventsAsync(api);
        }

        private static async Task TestCreateEventsAsync(ITimelineService api)
        {
            // Create event.
            Console.WriteLine("Creating event");
            TimelineEvent evt = new TimelineEvent
            {
                Title = "Test Event Title",
                Description = "Test description",
                EventDateTime = DateTime.Now,
                Longitude = -1.1234,
                Latitude = 1.1234
            };
            evt = await evt.CreateAsync(api);
            string id = evt.Id;
            DisplayTimelineEvent(evt);

            // Fetch event.
            Console.WriteLine();
            Console.WriteLine("Getting event");
            evt = await TimelineEvent.GetTimelineEventAsync(api, evt.Id);
            DisplayTimelineEvent(evt);

            // Link 
            Console.WriteLine();
            Console.WriteLine("Link event");
            Timeline timeline = await Timeline.GetTimelineAsync(api, "255c6ab0-79bc-4d2d-8793-bd508c7c39f9");
            await timeline.LinkEventAsync(api, evt);
            Console.WriteLine("Done");

            // Get linked events
            Console.WriteLine();
            Console.WriteLine("Get linked events");
            IList<LinkedEvent> linkedEvents = await timeline.GetEventsAsync(api);
            Console.WriteLine("Done");

            Console.WriteLine();
            Console.WriteLine("Getting events");
            foreach (var link in linkedEvents)
            {
                TimelineEvent timelineEvent = await TimelineEvent.GetTimelineEventAsync(api, link);
                DisplayTimelineEvent(timelineEvent);
            }

            // Unlink
            Console.WriteLine();
            Console.WriteLine("Unlink event");
            await timeline.UnlinkEventAsync(api, evt);
            Console.WriteLine("Done");

            // Delete event.
            Console.WriteLine();
            Console.WriteLine("Deleteing event...");
            await evt.DeleteAsync(api);
            Console.WriteLine("Done");
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

        private static async Task TestGetTimelinesAsync(ITimelineService api)
        {
            // Get the timelines associated with this API object.
            IList<Timeline> timelines = await Timeline.GetTimelinesAsync(api);

            // Display timelines.
            Console.WriteLine("Display list of all timelines");
            foreach (Timeline timeline in timelines)
            {
                DisplayTimeline(timeline);
            }
            Console.WriteLine();
        }

        private static async Task TestTimelineActionsAsync(ITimelineService api)
        {
            Console.WriteLine("Create timeline");
            Timeline timeline = await Timeline.CreateAsync(api, "Test Timeline");
            DisplayTimeline(timeline);
            string id = timeline.Id; // Store ID

            Console.WriteLine("Edit timeline");
            timeline.Title = "Edited Title";
            await timeline.EditTitleAsync(api);

            // Get and display timeline again.
            timeline = await Timeline.GetTimelineAsync(api, id);
            DisplayTimeline(timeline);

            // Remove timeline.
            Console.WriteLine("Delete timeline");
            await timeline.DeleteAsync(api);
            Console.WriteLine("Done");
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
