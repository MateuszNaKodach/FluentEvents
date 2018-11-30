using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus
{
    public class TopicEventReceiver : ITopicEventReceiver
    {
        private readonly ITopicEventReceiverConfig m_Config;
        private readonly ILogger<TopicEventReceiver> m_Logger;
        private readonly IPublishingService m_PublishingService;
        private readonly IEventsSerializationService m_EventsSerializationService;

        private SubscriptionClient m_SubscriptionClient;

        public TopicEventReceiver(
            ILogger<TopicEventReceiver> logger,
            IOptions<TopicEventReceiverConfig> config,
            IPublishingService publishingService,
            IEventsSerializationService eventsSerializationService
        )
        {
            if (config.Value.SubscriptionNameGenerator == null)
                throw new SubscriptionNameGeneratorIsNullException();
            if (config.Value.ReceiveConnectionString == null)
                throw new ReceiveConnectionStringIsNullException();
            if (config.Value.ManagementConnectionString == null)
                throw new ManagementConnectionStringIsNullException();
            if (config.Value.TopicPath == null)
                throw new TopicPathIsNullException();

            m_Config = config.Value;
            m_Logger = logger;
            m_PublishingService = publishingService;
            m_EventsSerializationService = eventsSerializationService;
        }

        public async Task StartReceivingAsync(CancellationToken cancellationToken = default)
        {
            var managementClient = new ManagementClient(m_Config.ManagementConnectionString);
            var subscriptionName = m_Config.SubscriptionNameGenerator.Invoke();

            await managementClient.CreateSubscriptionAsync(new SubscriptionDescription(m_Config.TopicPath, subscriptionName)
            {
                AutoDeleteOnIdle = m_Config.SubscriptionsAutoDeleteOnIdleTimeout
            }, cancellationToken);

            m_SubscriptionClient = new SubscriptionClient(
                new ServiceBusConnectionStringBuilder(m_Config.ReceiveConnectionString),
                subscriptionName
            );

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = m_Config.MaxConcurrentMessages,
                AutoComplete = true
            };

            m_SubscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                var stringMessageBody = m_Config.Encoding.GetString(message.Body);

                var entityEvent = m_EventsSerializationService.Deserialize(stringMessageBody);

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
