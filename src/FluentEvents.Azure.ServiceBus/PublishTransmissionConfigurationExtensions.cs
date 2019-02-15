using FluentEvents.Pipelines.Publication;

namespace FluentEvents.Azure.ServiceBus
{
    public static class PublishTransmissionConfigurationExtensions
    {
        public static IPublishTransmissionConfiguration WithAzureTopic(
            this IGlobalPublishingOptionsFactory globalPublishingOptionsFactory
        )
        {
            return globalPublishingOptionsFactory.With<TopicEventSender>();
        }
    }
}
