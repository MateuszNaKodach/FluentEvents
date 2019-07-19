using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     Provides APIs to publish events.
    /// </summary>
    public interface IPublishingService
    {
        /// <summary>
        ///     Publish an events to all the scoped subscriptions.
        /// </summary>
        /// <param name="pipelineEvent">The event to publish.</param>
        /// <param name="eventsScope">The scope of the event and the subscriptions.</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="eventsScope"/> is <see langword="null"/>
        /// </exception>
        Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, IEventsScope eventsScope);

        /// <summary>
        ///     Publishes an event to all the global subscriptions.
        /// </summary>
        /// <param name="pipelineEvent"> event to publish.</param>
        /// <returns>An awaitable task.</returns>
        Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent);
    }
}