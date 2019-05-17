using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class PublishingService : IPublishingService
    {
        private readonly ILogger<PublishingService> _logger;
        private readonly IGlobalSubscriptionCollection _globalSubscriptionCollection;
        private readonly ISubscriptionsMatchingService _subscriptionsMatchingService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PublishingService(
            ILogger<PublishingService> logger,
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            ISubscriptionsMatchingService subscriptionsMatchingService
        )
        {
            _logger = logger;
            _globalSubscriptionCollection = globalSubscriptionCollection;
            _subscriptionsMatchingService = subscriptionsMatchingService;
        }

        /// <inheritdoc />
        public async Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            await PublishInternalAsync(pipelineEvent, eventsScope.GetSubscriptions());
        }

        /// <inheritdoc />
        public async Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent)
            => await PublishInternalAsync(pipelineEvent, _globalSubscriptionCollection.GetGlobalScopeSubscriptions());

        private async Task PublishInternalAsync(PipelineEvent pipelineEvent, IEnumerable<Subscription> subscriptions)
        {
            _logger.PublishingEvent(pipelineEvent);

            var eventsSubscriptions = _subscriptionsMatchingService
                .GetMatchingSubscriptionsForSender(subscriptions, pipelineEvent.OriginalSender);

            foreach (var eventsSubscription in eventsSubscriptions)
            {
                try
                {
                    await eventsSubscription.PublishEventAsync(pipelineEvent);
                }
                catch (SubscriptionPublishAggregateException ex)
                {
                    foreach (var innerException in ex.InnerExceptions)
                        _logger.EventHandlerThrew(innerException);
                }
            }
        }
    }
}
