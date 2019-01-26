using System.Collections.Generic;

namespace FluentEvents.Queues
{
    public interface IEventQueueCollection : IEnumerable<IEventsQueue>
    {
        IEventsQueue GetOrAddEventsQueue(IInfrastructureEventsContext eventsContext, string queueName);
    }
}