namespace Echelon.TimelineApi
{
    public class LinkedEvent
    {
        public string Id { get; set; }
        public string TimelineEventId { get; set; }
        public string TimelineId { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantId { get; set; }
    }
}
