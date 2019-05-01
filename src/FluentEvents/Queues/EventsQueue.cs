using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal class EventsQueue : IEventsQueue
    {
        public string Name { get; }
        private readonly ConcurrentQueue<QueuedPipelineEvent> _queuedPipelineEvents;

        internal EventsQueue(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _queuedPipelineEvents = new ConcurrentQueue<QueuedPipelineEvent>();
        }

        public void DiscardQueuedEvents()
        {
            while (_queuedPipelineEvents.TryDequeue(out _))
            {
            }
        }

        public void Enqueue(QueuedPipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            _queuedPipelineEvents.Enqueue(pipelineEvent);
        }

        public IEnumerable<QueuedPipelineEvent> DequeueAll()
        {
            while (_queuedPipelineEvents.TryDequeue(out var pipelineEvent))
                yield return pipelineEvent;
        }
    }
}
