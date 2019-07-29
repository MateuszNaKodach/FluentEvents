using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal class EventsScopeQueuesFeature : IEventsScopeQueuesFeature
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, IEventsQueue>> _eventsQueues;

        public EventsScopeQueuesFeature()
        {
            _eventsQueues = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, IEventsQueue>>();
        }

        public IEnumerable<IEventsQueue> GetEventsQueues(Guid contextGuid)
        {
            if  (_eventsQueues.TryGetValue(contextGuid, out var eventsQueues))
                foreach (var eventsQueue in eventsQueues.Values)
                    yield return eventsQueue;
        }

        public IEventsQueue GetOrAddEventsQueue(Guid contextGuid, string queueName)
        {
            var queues = _eventsQueues.GetOrAdd(contextGuid, x => new ConcurrentDictionary<string, IEventsQueue>());
            return queues.GetOrAdd(queueName, x => new EventsQueue(queueName));
        }
    }
}