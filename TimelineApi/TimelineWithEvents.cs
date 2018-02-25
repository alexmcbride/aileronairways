using System.Collections.Generic;

namespace Echelon.TimelineApi
{
    public class TimelineWithEvents : Timeline
    {
        public List<TimelineEvent> TimelineEvents { get; set; }
    }
}
