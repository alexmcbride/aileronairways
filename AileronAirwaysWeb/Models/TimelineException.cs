using System;
using System.Runtime.Serialization;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// An exception that can be thrown by the timeline code.
    /// </summary>
    [Serializable]
    public class TimelineException : Exception
    {
        public TimelineException()
        {
        }

        public TimelineException(string message) : base(message)
        {
        }

        public TimelineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TimelineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}