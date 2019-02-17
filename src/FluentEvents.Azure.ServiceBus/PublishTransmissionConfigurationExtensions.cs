using FluentEvents.Pipelines.Publication;

namespace FluentEvents.Azure.ServiceBus
{
    public static class PublishTransmissionConfigurationExtensions
    {
        public static IPublishTransmissionConfiguration WithAzureTopic(
            this IConfigureTransmission configureTransmission
        )
        {
            return configureTransmission.With<TopicEventSender>();
        }
    }
}
