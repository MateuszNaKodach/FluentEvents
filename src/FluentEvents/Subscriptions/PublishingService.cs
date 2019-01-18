using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    public class PublishingService : IPublishingService
    {
        private readonly ILogger<PublishingService> m_Logger;
        private readonly IGlobalSubscriptionCollection m_GlobalSubscriptionCollection;
        private readonly ISubscriptionsMatchingService m_SubscriptionsMatchingService;

        public PublishingService(
            ILogger<PublishingService> logger,
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            ISubscriptionsMatchingService subscriptionsMatchingService
        )
        {
            m_Logger = logger;
            m_GlobalSubscriptionCollection = globalSubscriptionCollection;
            m_SubscriptionsMatchingService = subscriptionsMatchingService;
        }

        public async Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            await PublishInternalAsync(pipelineEvent, eventsScope.GetSubscriptions());
        }

        public async Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent)
            => await PublishInternalAsync(pipelineEvent, m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions());

        private async Task PublishInternalAsync(PipelineEvent pipelineEvent, IEnumerable<Subscription> subscriptions)
        {
            m_Logger.PublishingEvent(pipelineEvent);

            var eventsSubscriptions = m_SubscriptionsMatchingService
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
                        m_Logger.EventHandlerThrew(innerException);
                }
            }
        }
    }
}
