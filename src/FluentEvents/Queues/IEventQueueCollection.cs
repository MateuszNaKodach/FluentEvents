using System.Collections.Generic;

namespace FluentEvents.Queues
{
    public interface IEventQueueCollection : IEnumerable<IEventsQueue>
    {
        IEventsQueue GetOrAddEventsQueue(EventsQueuesContext eventsQueuesContext, string queueName);
    }
}