using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class TopicEventSender : EventSenderBase
    {
        public TopicEventSender(
            ILogger<TopicEventSender> logger,
            IEventsSerializationService eventsSerializationService,
            IOptions<TopicEventSenderConfig> config,
            ITopicClientFactory topicClientFactory
        ) : base(logger, eventsSerializationService, topicClientFactory.GetNew(config.Value.SendConnectionString))
        {
        }
    }
}