using System;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// Class to represent an API event, such as the API going offline.
    /// </summary>
    public class ApiEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
