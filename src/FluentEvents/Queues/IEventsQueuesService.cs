using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal interface IEventsQueuesService
    {
        Task ProcessQueuedEventsAsync(IEventsScope eventsScope, string queueName);
        void DiscardQueuedEvents(IEventsScope eventsScope, string queueName);
        void EnqueueEvent(IEventsScope eventsScope, PipelineEvent pipelineEvent, string queueName, Func<Task> invokeNextModule);
    }
}