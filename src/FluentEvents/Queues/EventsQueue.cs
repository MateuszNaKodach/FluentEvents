using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    public class EventsQueue : IEventsQueue
    {
        public string Name { get; }
        private readonly ConcurrentQueue<QueuedPipelineEvent> m_QueuedPipelineEvents;

        public EventsQueue(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            m_QueuedPipelineEvents = new ConcurrentQueue<QueuedPipelineEvent>();
        }

        public void DiscardQueuedEvents()
        {
            while (m_QueuedPipelineEvents.TryDequeue(out _))
            {
            }
        }

        public void Enqueue(QueuedPipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            m_QueuedPipelineEvents.Enqueue(pipelineEvent);
        }

        public IEnumerable<QueuedPipelineEvent> DequeueAll()
        {
            while (m_QueuedPipelineEvents.TryDequeue(out var pipelineEvent))
                yield return pipelineEvent;
        }
    }
}
