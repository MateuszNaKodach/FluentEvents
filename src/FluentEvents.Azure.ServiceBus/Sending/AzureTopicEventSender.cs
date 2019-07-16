using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class AzureTopicEventSender : IEventSender, IDisposable
    {
        private readonly ILogger<AzureTopicEventSender> _logger;
        private readonly IEventsSerializationService _eventsSerializationService;
        private readonly ITopicClient _topicClient;

        public AzureTopicEventSender(
            ILogger<AzureTopicEventSender> logger,
            IOptions<AzureTopicEventSenderConfig> config, 
            IEventsSerializationService eventsSerializationService,
            ITopicClientFactory topicClientFactory
        )
        {
            _logger = logger;
            _eventsSerializationService = eventsSerializationService;
            _topicClient = topicClientFactory.GetNew(config.Value.SendConnectionString);
        }

        public async Task SendAsync(PipelineEvent pipelineEvent)
        {
            var serializedEvent = _eventsSerializationService.SerializeEvent(pipelineEvent);
            var message = new Message(serializedEvent)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await _topicClient.SendAsync(message).ConfigureAwait(false);

            _logger.MessageSent(message.MessageId);
        }

        public void Dispose()
        {
            _topicClient.CloseAsync().GetAwaiter().GetResult();
        }
    }
}