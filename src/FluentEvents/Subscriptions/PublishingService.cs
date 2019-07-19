using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    internal class PublishingService : IPublishingService
    {
        private readonly ILogger<PublishingService> _logger;
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;
        private readonly ISubscriptionsMatchingService _subscriptionsMatchingService;

        public PublishingService(
            ILogger<PublishingService> logger,
            IGlobalSubscriptionsService globalSubscriptionsService,
            IScopedSubscriptionsService scopedSubscriptionsService,
            ISubscriptionsMatchingService subscriptionsMatchingService
        )
        {
            _logger = logger;
            _globalSubscriptionsService = globalSubscriptionsService;
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _subscriptionsMatchingService = subscriptionsMatchingService;
        }

        public Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, IEventsScope eventsScope)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            var subscriptions = eventsScope.GetSubscriptionsFeature().GetSubscriptions(_scopedSubscriptionsService);

            return PublishInternalAsync(pipelineEvent, subscriptions);
        }

        public Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent)
            => PublishInternalAsync(pipelineEvent, _globalSubscriptionsService.GetGlobalSubscriptions());

        private async Task PublishInternalAsync(PipelineEvent pipelineEvent, IEnumerable<Subscription> subscriptions)
        {
            _logger.PublishingEvent(pipelineEvent);

            var eventsSubscriptions = _subscriptionsMatchingService
                .GetMatchingSubscriptionsForEvent(subscriptions, pipelineEvent.Event);

            var exceptions = new List<Exception>();

            foreach (var eventsSubscription in eventsSubscriptions)
            {
                var exception = await eventsSubscription.PublishEventAsync(pipelineEvent).ConfigureAwait(false);

                if (exception != null)
                {
                    exceptions.Add(exception);
                    _logger.EventHandlerThrew(exception);
                }
            }

            if (exceptions.Any())
                throw new SubscriptionPublishAggregateException(exceptions);
        }
    }
}
