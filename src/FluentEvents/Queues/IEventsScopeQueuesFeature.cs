using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal interface IEventsScopeQueuesFeature
    {
        IEnumerable<IEventsQueue> GetEventsQueues(IEventsContext eventsContext);
        IEventsQueue GetOrAddEventsQueue(IEventsContext eventsContext, string queueName);
    }
}