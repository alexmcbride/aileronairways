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
            await TestGetTimelinesAsync(api);
            await TestTimelineActionsAsync(api);
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
