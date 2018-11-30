using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus
{
    internal class TopicEventSender : ITopicEventSender
    {
        private readonly ILogger<TopicEventSender> m_Logger;
        private readonly IEventsSerializationService m_EventsSerializationService;
        private readonly ITopicEventSenderConfig m_Config;
        private readonly TopicClient m_TopicClient;

        public TopicEventSender(
            ILogger<TopicEventSender> logger,
            IOptions<TopicEventSenderConfig> config, 
            IEventsSerializationService eventsSerializationService
        )
        {
            m_Logger = logger;
            m_EventsSerializationService = eventsSerializationService;
            m_Config = config.Value;
            m_TopicClient = new TopicClient(new ServiceBusConnectionStringBuilder(m_Config.ConnectionString));
        }

        public void Dispose()
        {
            m_TopicClient.CloseAsync().GetAwaiter().GetResult();
        }

        public async Task SendAsync(PipelineEvent pipelineEvent)
        {
            var serializedEvent = m_EventsSerializationService.SerializeEvent(pipelineEvent);
            var message = new Message(m_Config.Encoding.GetBytes(serializedEvent))
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await m_TopicClient.SendAsync(message);

            m_Logger.MessageSent(message.MessageId);
        }
    }
}