using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public interface IEventsQueuesService
    {
        Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName);
        void DiscardQueuedEvents(EventsScope eventsScope, string queueName);
        void EnqueueEvent(EventsScope eventsScope, PipelineEvent pipelineEvent, IPipeline pipeline);
    }
}