using FluentEvents.Pipelines.Publication;

namespace FluentEvents.Azure.ServiceBus
{
    public static class SenderTypeConfigurationExtensions
    {
        public static ISenderTypeConfiguration WithAzureTopic(this IGlobalPublishingOptionsFactory globalPublishingOptionsFactory)
        {
            return globalPublishingOptionsFactory.With<TopicEventSender>();
        }
    }
}
