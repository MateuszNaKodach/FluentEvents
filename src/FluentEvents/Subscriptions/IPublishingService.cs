using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    public interface IPublishingService
    {
        Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
        Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent);
    }
}