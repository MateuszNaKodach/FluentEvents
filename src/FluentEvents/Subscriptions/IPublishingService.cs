using System;
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
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="pipelineEvent"/> and/or <paramref name="eventsScope"/> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="SubscriptionPublishAggregateException">
        ///     One or more event handlers threw an exception.
        /// </exception>
        Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, IEventsScope eventsScope);

        /// <summary>
        ///     Publishes an event to all the global subscriptions.
        /// </summary>
        /// <param name="pipelineEvent">The event to publish.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="pipelineEvent"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SubscriptionPublishAggregateException">
        ///     One or more event handlers threw an exception.
        /// </exception>
        Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent);
    }
}