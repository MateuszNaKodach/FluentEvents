using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class TopicEventReceiver : IEventReceiver
    {
        private readonly TopicEventReceiverConfig m_Config;
        private readonly ILogger<TopicEventReceiver> m_Logger;
        private readonly IPublishingService m_PublishingService;
        private readonly IEventsSerializationService m_EventsSerializationService;
        private readonly ITopicSubscriptionsService m_TopicSubscriptionsService;
        private readonly ISubscriptionClientFactory m_SubscriptionClientFactory;

        private ISubscriptionClient m_SubscriptionClient;

        public TopicEventReceiver(
            ILogger<TopicEventReceiver> logger,
            IOptions<TopicEventReceiverConfig> config,
            IPublishingService publishingService,
            IEventsSerializationService eventsSerializationService,
            ITopicSubscriptionsService topicSubscriptionsService,
            ISubscriptionClientFactory subscriptionClientFactory
        )
        {
            m_Config = config.Value;
            m_Logger = logger;
            m_PublishingService = publishingService;
            m_EventsSerializationService = eventsSerializationService;
            m_TopicSubscriptionsService = topicSubscriptionsService;
            m_SubscriptionClientFactory = subscriptionClientFactory;
        }

        public async Task StartReceivingAsync(CancellationToken cancellationToken = default)
        {
            var subscriptionName = m_Config.SubscriptionNameGenerator.Invoke();

            await m_TopicSubscriptionsService.CreateSubscriptionAsync(
                m_Config.ManagementConnectionString,
                subscriptionName,
                m_Config.TopicPath,
                m_Config.SubscriptionsAutoDeleteOnIdleTimeout,
                cancellationToken
            );

            m_SubscriptionClient = m_SubscriptionClientFactory.GetNew(
                m_Config.ReceiveConnectionString,
                subscriptionName
            );

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = m_Config.MaxConcurrentMessages,
                AutoComplete = true
            };

            m_SubscriptionClient.RegisterMessageHandler(HandleMessageAsync, messageHandlerOptions);
        }

        private async Task HandleMessageAsync(Message message, CancellationToken token)
        {
            try
            {
                var entityEvent = m_EventsSerializationService.DeserializeEvent(message.Body);

                await m_PublishingService.PublishEventToGlobalSubscriptionsAsync(entityEvent);
            }
            catch (Exception ex)
            {
                m_Logger.MessagesProcessingThrew(ex, message.MessageId);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            m_Logger.ServiceBusExceptionReceived(
                exceptionReceivedEventArgs.Exception,
                exceptionReceivedEventArgs.ExceptionReceivedContext
            );
            return Task.CompletedTask;
        }

        public async Task StopReceivingAsync(CancellationToken cancellationToken = default)
        {
            await m_SubscriptionClient.CloseAsync();
        }
    }
}
