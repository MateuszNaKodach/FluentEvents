using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal class EventsScopeQueuesFeature : IEventsScopeQueuesFeature
    {
        private readonly ConcurrentDictionary<IEventsContext, ConcurrentDictionary<string, IEventsQueue>> _eventsQueues;

        public EventsScopeQueuesFeature()
        {
            _eventsQueues = new ConcurrentDictionary<IEventsContext, ConcurrentDictionary<string, IEventsQueue>>();
        }

        public IEnumerable<IEventsQueue> GetEventsQueues(IEventsContext eventsContext)
        {
            if  (_eventsQueues.TryGetValue(eventsContext, out var eventsQueues))
                foreach (var eventsQueue in eventsQueues.Values)
                    yield return eventsQueue;
        }

        public IEventsQueue GetOrAddEventsQueue(IEventsContext eventsContext, string queueName)
        {
            var queues = _eventsQueues.GetOrAdd(eventsContext, x => new ConcurrentDictionary<string, IEventsQueue>());
            return queues.GetOrAdd(queueName, x => new EventsQueue(queueName));
        }
    }
}