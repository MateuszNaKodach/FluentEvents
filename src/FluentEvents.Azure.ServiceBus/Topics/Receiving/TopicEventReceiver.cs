using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    internal class TopicEventReceiver : EventReceiverBase
    {
        private readonly TopicEventReceiverConfig _config;
        private readonly ITopicSubscriptionsService _topicSubscriptionsService;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;

        public TopicEventReceiver(
            ILogger<TopicEventReceiver> logger,
            IOptions<TopicEventReceiverConfig> config,
            IPublishingService publishingService,
            IEventsSerializationService eventsSerializationService,
            ITopicSubscriptionsService topicSubscriptionsService,
            ISubscriptionClientFactory subscriptionClientFactory
        )
            : base(logger, eventsSerializationService, publishingService, config.Value)
        {
            _config = config.Value;
            _topicSubscriptionsService = topicSubscriptionsService;
            _subscriptionClientFactory = subscriptionClientFactory;
        }

        protected internal override async Task<IReceiverClient> CreateReceiverClientAsync(CancellationToken cancellationToken)
        {
            var subscriptionName = _config.SubscriptionNameGenerator.Invoke();

            await _topicSubscriptionsService.CreateSubscriptionAsync(
                _config.ManagementConnectionString,
                subscriptionName,
                _config.TopicPath,
                _config.SubscriptionsAutoDeleteOnIdleTimeout,
                cancellationToken
            ).ConfigureAwait(false);

            var subscriptionClient = _subscriptionClientFactory.GetNew(
                _config.ReceiveConnectionString,
                subscriptionName
            );

            return subscriptionClient;
        }
    }
}
