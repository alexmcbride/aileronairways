namespace Echelon.TimelineApi
{
    public class LinkedEvent : ModelBase
    {
        public string TimelineEventId { get; set; }
        public string TimelineId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
