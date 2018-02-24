using System;
using System.Collections.Generic;
using System.IO;

namespace Echelon.TimelineApi.TestConsole
{
    class Program
    {
        const string BaseUrl = "https://gcu.ideagen-development.com/";
        const string AuthToken = "5e906627-a997-4ab1-94c5-07fcb6c9383d";
        const string TenantId = "Team3";

        static void Main(string[] args)
        {
            //RunTestsAsync();
            //RunTestsLinkyStuffAsync();
            RunAttachmentTestsAsync();

            // Stop program from exiting.
            Console.ReadKey(true);
        }

        private static async void RunAttachmentTestsAsync()
        {
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            Console.WriteLine("Getting event");
            TimelineEvent evt = await TimelineEvent.GetTimelineEventAsync(api, "ID2");
            Console.WriteLine("Done");
            DisplayTimelineEvent(evt);

            Console.WriteLine("Creating attachment");
            Attachment attachment = await Attachment.CreateAsync(api, evt.Id, "Test3.txt");
            DisplayAttachment(attachment);
            Console.WriteLine("Done");

            Console.WriteLine("Uploading attachment");
            File.Copy("Test3.txt", attachment.Id);
            await attachment.UploadAsync(api, attachment.Id);
            Console.WriteLine("Done");

            Console.WriteLine("Downloading attachment");
            await attachment.DownloadAsync(api, @"C:\Users\alexm\Desktop");
            Console.WriteLine("Done");
        }

        private static void DisplayAttachment(Attachment attachment)
        {
            Console.WriteLine($"Id: {attachment.Id}");
            Console.WriteLine($"Title: {attachment.Title}");
            Console.WriteLine($"TimelineEventId: {attachment.TimelineEventId}");
            Console.WriteLine($"IsDeleted: {attachment.IsDeleted}");
            Console.WriteLine($"TenantId: {attachment.TenantId}");
            Console.WriteLine();
        }

        private static async void RunTestsAsync()
        {
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            // Get timeline with this ID.
            Timeline timeline = await Timeline.GetTimelineAsync(api, "255c6ab0-79bc-4d2d-8793-bd508c7c39f9");
            DisplayTimeline(timeline);

            // Create new event.
            //TimelineEvent evt = await TimelineEvent.CreateAsync(api, "New Event 18", "Event description", DateTime.Now, "-1.1234,1.1234");
            //DisplayTimelineEvent(evt);

            // Link timeline and event together.
            //await timeline.LinkEventAsync(api, evt);

            // Get list of linked events.
            IList<LinkedEvent> linkedEvents = await timeline.GetEventsAsync(api);

            // Wait for all TimelineEvent objects to download.
            foreach (LinkedEvent linkedEvent in linkedEvents)
            {
                TimelineEvent evt2 = await TimelineEvent.GetTimelineEventAsync(api, linkedEvent.TimelineEventId);
                DisplayTimelineEvent(evt2);
            }
        }

        private async static void RunTestsLinkyStuffAsync()
        {
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            Console.WriteLine("Fetch timelines");
            TimelineEvent a = await TimelineEvent.GetTimelineEventAsync(api, "8e8a526a-db39-4b41-a00c-e55308d3ef6f");
            TimelineEvent b = await TimelineEvent.GetTimelineEventAsync(api, "e97f9f95-7147-4782-8fbc-f445f343dd20");
            Console.WriteLine("Done");

            //Console.WriteLine("Link events");
            //await a.LinkTimelineEventsAsync(api, b);
            //Console.WriteLine("Done");

            //Console.WriteLine("Link events");
            //await a.UnlinkTimelineEventsAsync(api, b);
            //Console.WriteLine("Done");

            Console.WriteLine("Get linked events");
            var linkedEvents = await a.GetLinkedTimelineEventsAsync(api);
            Console.WriteLine("Done");

            Console.WriteLine("Fetching event for each link.");
            foreach (var linkedEvent in linkedEvents)
            {
                var evt = await TimelineEvent.GetTimelineEventAsync(api, linkedEvent.LinkedToTimelineEventId);
                DisplayTimelineEvent(evt);
            }
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
