using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Utils;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    public class PublishingService : IPublishingService
    {
        private readonly ILogger<PublishingService> m_Logger;
        private readonly IGlobalSubscriptionCollection m_GlobalSubscriptionCollection;

        public PublishingService(ILogger<PublishingService> logger, IGlobalSubscriptionCollection globalSubscriptionCollection)
        {
            m_Logger = logger;
            m_GlobalSubscriptionCollection = globalSubscriptionCollection;
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

            var types = pipelineEvent.OriginalSender.GetType().GetBaseTypesInclusive();

            var eventsSubscriptions = subscriptions
                .Where(x => types.Any(y => y == x.SourceType));

            foreach (var eventsSubscription in eventsSubscriptions)
            {
                try
                {
                    await eventsSubscription.PublishEvent(pipelineEvent);
                }
                catch (Exception ex)
                {
                    m_Logger.EventHandlerThrew(ex);
                }
            }
        }
    }
}
