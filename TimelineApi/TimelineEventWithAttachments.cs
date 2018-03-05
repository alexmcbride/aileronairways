using System.Collections.Generic;

namespace Echelon.TimelineApi
{
    public class TimelineEventWithAttachments : TimelineEvent
    {
        public List<Attachment> Attachments { get; set; }
    }
}
