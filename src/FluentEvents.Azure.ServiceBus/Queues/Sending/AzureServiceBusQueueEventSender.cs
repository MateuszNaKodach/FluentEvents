using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Azure.ServiceBus.Topics.Sending;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Sending
{
    internal class AzureServiceBusQueueEventSender : AzureServiceBusEventSenderBase
    {
        public AzureServiceBusQueueEventSender(
            ILogger<AzureServiceBusQueueEventSender> logger,
            IEventsSerializationService eventsSerializationService,
            IOptions<AzureServiceBusQueueEventSenderConfig> config,
            IQueueClientFactory queueClientFactory
        ) : base(logger, eventsSerializationService, queueClientFactory.GetNew(config.Value.SendConnectionString))
        {
        }
    }
}