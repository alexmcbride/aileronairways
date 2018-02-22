namespace Echelon.TimelineApi
{
    public class TimelineEventLink : ModelBase
    {
        public string TimelineEventId { get; set; }
        public string LinkedToTimelineEventId { get; set; }
    }
}
