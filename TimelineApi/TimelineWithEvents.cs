using System.Collections.Generic;

namespace Echelon.TimelineApi
{
    public class TimelineWithEvents : Timeline
    {
        public List<TimelineEventWithAttachments> TimelineEvents { get; set; }
    }
}
