using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class PublishingService : IPublishingService
    {
        private readonly ILogger<PublishingService> _logger;
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly ISubscriptionsMatchingService _subscriptionsMatchingService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PublishingService(
            ILogger<PublishingService> logger,
            IGlobalSubscriptionsService globalSubscriptionsService,
            ISubscriptionsMatchingService subscriptionsMatchingService
        )
        {
            _logger = logger;
            _globalSubscriptionsService = globalSubscriptionsService;
            _subscriptionsMatchingService = subscriptionsMatchingService;
        }

        /// <inheritdoc />
        public Task PublishEventToScopedSubscriptionsAsync(PipelineEvent pipelineEvent, EventsScope eventsScope)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            return PublishInternalAsync(pipelineEvent, eventsScope.GetSubscriptions());
        }

        /// <inheritdoc />
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
