using System;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal interface IEventsScopeQueuesFeature
    {
        IEnumerable<IEventsQueue> GetEventsQueues(Guid contextGuid);
        IEventsQueue GetOrAddEventsQueue(Guid contextGuid, string queueName);
    }
}