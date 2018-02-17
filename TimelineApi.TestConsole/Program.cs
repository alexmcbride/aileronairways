using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Echelon.TimelineApi.TestConsole
{
    class Program
    {
        const string BaseUrl = "https://gcu.ideagen-development.com/";
        const string AuthToken = "5e906627-a997-4ab1-94c5-07fcb6c9383d";
        const string TenantId = "Team3";

        static void Main(string[] args)
        {
            // Time how long operation takes.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create new API object and pass in our parameters.
            ITimelineService api = new TimelineService(BaseUrl, AuthToken, TenantId);

            // Test methods.
            TestGetTimelines(api);
            TestTimelineActions(api);

            // Output elapsed time.
            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

            // Stop program from exiting.
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void TestGetTimelines(ITimelineService api)
        {
            // Get the timelines associated with this API object.
            IList<Timeline> timelines = Timeline.GetTimelines(api);

            // Display timelines.
            Console.WriteLine("Display list of all timelines");
            foreach (Timeline timeline in timelines)
            {
                DisplayTimeline(timeline);
            }
            Console.WriteLine();
        }

        private static void TestTimelineActions(ITimelineService api)
        {
            Console.WriteLine("Create timeline");
            Timeline timeline = Timeline.Create(api, "Test Timeline");
            DisplayTimeline(timeline);
            string id = timeline.Id; // Store ID

            Console.WriteLine("Edit timeline");
            timeline.Title = "Edited Title";
            timeline.EditTitle(api);

            // Get and display timeline again.
            timeline = Timeline.GetTimeline(api, id);
            DisplayTimeline(timeline);

            // Remove timeline.
            Console.WriteLine("Delete timeline");
            timeline.Delete(api);
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
