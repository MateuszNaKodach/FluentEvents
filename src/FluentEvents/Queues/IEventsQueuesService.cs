using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public interface IEventsQueuesService
    {
        Task ProcessQueuedEventsAsync(EventsScope eventsScope, IEventsContext eventsContext, string queueName);
        void DiscardQueuedEvents(IEventsContext eventsContext, string queueName);
        void EnqueueEvent(PipelineEvent pipelineEvent, IPipeline pipeline);
    }
}