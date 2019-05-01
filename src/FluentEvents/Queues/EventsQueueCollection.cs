using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal class EventsQueueCollection : IEventsQueueCollection
    {
        private readonly ConcurrentDictionary<(EventsQueuesContext, string), IEventsQueue> _eventsQueues;

        public EventsQueueCollection()
        {
            _eventsQueues = new ConcurrentDictionary<(EventsQueuesContext, string), IEventsQueue>();
        }

        public IEventsQueue GetOrAddEventsQueue(EventsQueuesContext eventsQueuesContext, string queueName)
        {
            return _eventsQueues.GetOrAdd((eventsQueuesContext, queueName), x => new EventsQueue(queueName));
        }

        public IEnumerator<IEventsQueue> GetEnumerator()
        {
            return _eventsQueues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
