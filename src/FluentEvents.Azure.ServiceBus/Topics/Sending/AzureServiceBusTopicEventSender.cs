using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class AzureServiceBusTopicEventSender : AzureServiceBusEventSenderBase
    {
        public AzureServiceBusTopicEventSender(
            ILogger<AzureServiceBusTopicEventSender> logger,
            IEventsSerializationService eventsSerializationService,
            IOptions<AzureServiceBusTopicEventSenderConfig> config,
            ITopicClientFactory topicClientFactory
        ) : base(logger, eventsSerializationService, topicClientFactory.GetNew(config.Value.SendConnectionString))
        {
        }
    }
}