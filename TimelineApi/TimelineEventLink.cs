namespace Echelon.TimelineApi
{
    public class TimelineEventLink
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
    }
}
