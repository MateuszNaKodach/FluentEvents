using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class AzureTopicEventReceiver : IEventReceiver
    {
        private readonly AzureTopicEventReceiverOptions _options;
        private readonly ILogger<AzureTopicEventReceiver> _logger;
        private readonly IPublishingService _publishingService;
        private readonly IEventsSerializationService _eventsSerializationService;
        private readonly ITopicSubscriptionsService _topicSubscriptionsService;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;

        private ISubscriptionClient _subscriptionClient;

        public AzureTopicEventReceiver(
            ILogger<AzureTopicEventReceiver> logger,
            IOptions<AzureTopicEventReceiverOptions> config,
            IPublishingService publishingService,
            IEventsSerializationService eventsSerializationService,
            ITopicSubscriptionsService topicSubscriptionsService,
            ISubscriptionClientFactory subscriptionClientFactory
        )
        {
            _options = config.Value;
            _logger = logger;
            _publishingService = publishingService;
            _eventsSerializationService = eventsSerializationService;
            _topicSubscriptionsService = topicSubscriptionsService;
            _subscriptionClientFactory = subscriptionClientFactory;
        }

        public async Task StartReceivingAsync(CancellationToken cancellationToken = default)
        {
            var subscriptionName = _options.SubscriptionName ?? _options.SubscriptionNameGenerator.Invoke();

            if (_options.IsSubscriptionCreationEnabled)
            {
                await _topicSubscriptionsService.CreateSubscriptionAsync(
                    _options.ManagementConnectionString,
                    subscriptionName,
                    _options.TopicPath,
                    _options.SubscriptionsAutoDeleteOnIdleTimeout,
                    cancellationToken
                ).ConfigureAwait(false);
            }

            _subscriptionClient = _subscriptionClientFactory.GetNew(
                _options.ReceiveConnectionString,
                subscriptionName
            );

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = _options.MaxConcurrentMessages,
                AutoComplete = true
            };

            _subscriptionClient.RegisterMessageHandler(HandleMessageAsync, messageHandlerOptions);
        }

        private async Task HandleMessageAsync(Message message, CancellationToken token)
        {
            try
            {
                var entityEvent = _eventsSerializationService.DeserializeEvent(message.Body);

                await _publishingService.PublishEventToGlobalSubscriptionsAsync(entityEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.MessagesProcessingThrew(ex, message.MessageId);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.ServiceBusExceptionReceived(
                exceptionReceivedEventArgs.Exception,
                exceptionReceivedEventArgs.ExceptionReceivedContext
            );
            return Task.CompletedTask;
        }

        public Task StopReceivingAsync(CancellationToken cancellationToken = default)
        {
            return _subscriptionClient.CloseAsync();
        }
    }
}
