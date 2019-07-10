using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal abstract class EventSenderBase : IEventSender, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IEventsSerializationService _eventsSerializationService;
        private readonly ISenderClient _senderClient;

        protected EventSenderBase(
            ILogger logger, 
            IEventsSerializationService eventsSerializationService,
            ISenderClient senderClient
        )
        {
            _logger = logger;
            _eventsSerializationService = eventsSerializationService;
            _senderClient = senderClient;
        }

        public async Task SendAsync(PipelineEvent pipelineEvent)
        {
            var serializedEvent = _eventsSerializationService.SerializeEvent(pipelineEvent);
            var message = new Message(serializedEvent)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await _senderClient.SendAsync(message).ConfigureAwait(false);

            _logger.MessageSent(message.MessageId);
        }

        public void Dispose()
        {
            _senderClient.CloseAsync().GetAwaiter().GetResult();
        }
    }
}