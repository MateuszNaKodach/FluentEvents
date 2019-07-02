using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Receiving
{
    internal class AzureServiceBusQueueEventReceiver : AzureServiceBusEventReceiverBase
    {
        private readonly IQueueClientFactory _queueClientFactory;
        private readonly AzureServiceBusQueueEventReceiverConfig _config;

        public AzureServiceBusQueueEventReceiver(
            IQueueClientFactory queueClientFactory,
            ILogger<AzureServiceBusQueueEventReceiver> logger,
            IEventsSerializationService eventsSerializationService,
            IPublishingService publishingService,
            IOptions<AzureServiceBusQueueEventReceiverConfig> config
        ) : base(logger, eventsSerializationService, publishingService, config.Value)
        {
            _queueClientFactory = queueClientFactory;
            _config = config.Value;
        }

        protected override Task<IReceiverClient> CreateReceiverClient(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReceiverClient>(_queueClientFactory.GetNew(_config.ReceiveConnectionString));
        }
    }
}
