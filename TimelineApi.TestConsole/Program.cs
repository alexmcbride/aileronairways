using System;
using System.Collections.Generic;

namespace Echelon.TimelineApi.TestConsole
{
    class Program
    {
        public const string BaseUrl = "https://gcu.ideagen-development.com/";
        public const string AuthToken = "5e906627-a997-4ab1-94c5-07fcb6c9383d";
        public const string TenantId = "Team3";

        static void Main(string[] args)
        {
            IWebClientHelper client = new WebClientHelper();

            // Create new API object and set some parameters.
            ITimelineService api = new TimelineService(client, BaseUrl, AuthToken, TenantId);

            // Get the timelines from this API.
            IList<Timeline> timelines = Timeline.GetTimelines(api);

            // Display timelines.
            foreach (Timeline timeline in timelines)
            {
                Console.WriteLine("Timeline:");
                Console.WriteLine($"ID: {timeline.Id}");
                Console.WriteLine($"Title: {timeline.Title}");
                Console.WriteLine($"Creation: {timeline.CreationTimeStamp}");
                Console.WriteLine($"Is Deleted: {timeline.IsDeleted}");
                Console.WriteLine($"Tenant ID: {timeline.TenantId}");
                Console.Write("----");
            }

            // Stop program from exiting.
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
