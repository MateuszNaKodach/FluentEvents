using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal interface IEventsQueuesService
    {
        Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName);
        void DiscardQueuedEvents(EventsScope eventsScope, string queueName);
        void EnqueueEvent(EventsScope eventsScope, PipelineEvent pipelineEvent, string queueName, Func<Task> invokeNextModule);
    }
}