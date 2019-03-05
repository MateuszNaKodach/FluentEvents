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
    internal class TopicEventReceiver : IEventReceiver
    {
        private readonly TopicEventReceiverConfig m_Config;
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
            try
            {
                var managementClient = new ManagementClient(m_Config.ManagementConnectionString);
                var subscriptionName = m_Config.SubscriptionNameGenerator.Invoke();

                await managementClient.CreateSubscriptionAsync(
                    new SubscriptionDescription(m_Config.TopicPath, subscriptionName)
                    {
                        AutoDeleteOnIdle = m_Config.SubscriptionsAutoDeleteOnIdleTimeout
                    },
                    cancellationToken
                );

                m_SubscriptionClient = new SubscriptionClient(
                    new ServiceBusConnectionStringBuilder(m_Config.ReceiveConnectionString),
                    subscriptionName
                );

                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = m_Config.MaxConcurrentMessages,
                    AutoComplete = true
                };

                m_SubscriptionClient.RegisterMessageHandler(HandleMessageAsync, messageHandlerOptions);
            }
            catch (ServiceBusException e)
            {
                throw new TopicEventReceiverStartException(e);
            }
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
