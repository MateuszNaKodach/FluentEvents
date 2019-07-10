using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Sending
{
    internal class QueueEventSender : EventSenderBase
    {
        public QueueEventSender(
            ILogger<QueueEventSender> logger,
            IEventsSerializationService eventsSerializationService,
            IOptions<QueueEventSenderConfig> config,
            IQueueClientFactory queueClientFactory
        ) : base(logger, eventsSerializationService, queueClientFactory.GetNew(config.Value.SendConnectionString))
        {
        }
    }
}