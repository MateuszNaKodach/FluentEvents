using System.Collections.Generic;

namespace FluentEvents.Queues
{
    internal interface IEventsQueue
    {
        void DiscardQueuedEvents();
        void Enqueue(QueuedPipelineEvent pipelineEvent);
        IEnumerable<QueuedPipelineEvent> DequeueAll();
        string Name { get; }
    }
}