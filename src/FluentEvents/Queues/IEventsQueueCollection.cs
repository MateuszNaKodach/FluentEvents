using System.Collections.Generic;

namespace FluentEvents.Queues
{
    public interface IEventsQueueCollection : IEnumerable<IEventsQueue>
    {
        IEventsQueue GetOrAddEventsQueue(EventsQueuesContext eventsQueuesContext, string queueName);
    }
}