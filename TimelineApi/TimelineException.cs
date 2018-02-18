using System;
using System.Runtime.Serialization;

namespace Echelon.TimelineApi
{
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