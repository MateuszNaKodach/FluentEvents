using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class TopicEventSender : IEventSender, IDisposable
    {
        private readonly ILogger<TopicEventSender> m_Logger;
        private readonly IEventsSerializationService m_EventsSerializationService;
        private readonly ITopicClient m_TopicClient;

        public TopicEventSender(
            ILogger<TopicEventSender> logger,
            IOptions<TopicEventSenderConfig> config, 
            IEventsSerializationService eventsSerializationService,
            ITopicClientFactory topicClientFactory
        )
        {
            m_Logger = logger;
            m_EventsSerializationService = eventsSerializationService;
            m_TopicClient = topicClientFactory.GetNew(config.Value.ConnectionString);
        }

        public async Task SendAsync(PipelineEvent pipelineEvent)
        {
            var serializedEvent = m_EventsSerializationService.SerializeEvent(pipelineEvent);
            var message = new Message(serializedEvent)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await m_TopicClient.SendAsync(message);

            m_Logger.MessageSent(message.MessageId);
        }

        public void Dispose()
        {
            m_TopicClient.CloseAsync().GetAwaiter().GetResult();
        }
    }
}