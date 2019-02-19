using System;

namespace FluentEvents.Queues
{
    internal class EventsQueuesContext
    {
        public Guid Guid { get; } = Guid.NewGuid();
    }
}
