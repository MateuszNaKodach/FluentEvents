using System;
using System.Runtime.Serialization;

namespace FluentEvents
{
    [Serializable]
    public class FluentEventsException : Exception
    {
        public FluentEventsException()
        {
        }

        public FluentEventsException(string message) : base(message)
        {
        }

        public FluentEventsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FluentEventsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
