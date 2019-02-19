using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal class EventsQueueCollection : IEventsQueueCollection
    {
        private readonly ConcurrentDictionary<(EventsQueuesContext, string), IEventsQueue> m_EventsQueues;

        public EventsQueueCollection()
        {
            m_EventsQueues = new ConcurrentDictionary<(EventsQueuesContext, string), IEventsQueue>();
        }

        public IEventsQueue GetOrAddEventsQueue(EventsQueuesContext eventsQueuesContext, string queueName)
        {
            return m_EventsQueues.GetOrAdd((eventsQueuesContext, queueName), x => new EventsQueue(queueName));
        }

        public IEnumerator<IEventsQueue> GetEnumerator()
        {
            return m_EventsQueues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
