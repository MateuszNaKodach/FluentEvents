using System;

namespace FluentEvents.Queues
{
    public class EventsQueuesContext
    {
        public Guid Guid { get; } = Guid.NewGuid();
    }
}
