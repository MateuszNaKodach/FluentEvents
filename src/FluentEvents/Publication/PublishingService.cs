﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Publication
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
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            var subscriptions = eventsScope.GetSubscriptionsFeature().GetSubscriptions(_scopedSubscriptionsService);

            return PublishInternalAsync(pipelineEvent, subscriptions);
        }

        public Task PublishEventToGlobalSubscriptionsAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));

            return PublishInternalAsync(pipelineEvent, _globalSubscriptionsService.GetGlobalSubscriptions());
        }

        private async Task PublishInternalAsync(PipelineEvent pipelineEvent, IEnumerable<Subscription> subscriptions)
        {
            _logger.PublishingEvent(pipelineEvent);

            var eventsSubscriptions = _subscriptionsMatchingService
                .GetMatchingSubscriptionsForEvent(subscriptions, pipelineEvent.Event);

            var exceptions = new List<Exception>();

            foreach (var eventsSubscription in eventsSubscriptions)
            {
                try
                {
                    await eventsSubscription.InvokeEventsHandlerAsync(pipelineEvent).ConfigureAwait(false);
                }
                catch (SubscribedEventHandlerThrewException ex)
                {
                    exceptions.Add(ex.InnerException);
                    _logger.EventHandlerThrew(ex.InnerException);
                }
            }

            if (exceptions.Any())
                throw new PublicationAggregateException(exceptions);
        }
    }
}