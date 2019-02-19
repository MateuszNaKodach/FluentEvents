using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal interface IEventsQueueCollection : IEnumerable<IEventsQueue>
    {
        IEventsQueue GetOrAddEventsQueue(EventsQueuesContext eventsQueuesContext, string queueName);
    }
}