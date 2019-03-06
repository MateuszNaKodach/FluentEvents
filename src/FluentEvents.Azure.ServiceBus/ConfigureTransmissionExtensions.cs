using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Pipelines.Publication;

namespace FluentEvents.Azure.ServiceBus
{
    /// <summary>
    ///     Extension methods for <see cref="IPublishTransmissionConfiguration"/>.
    /// </summary>
    public static class ConfigureTransmissionExtensions
    {
        /// <summary>
        ///     Publishes the event to different instances of the application
        ///     with an Azure Service Bus topic.
        /// </summary>
        /// <remarks>
        ///     This method can be used if the AzureTopicSender plugin is added to the <see cref="EventsContext"/>.
        /// </remarks>
        /// <param name="configureTransmission">
        ///     The <see cref="IConfigureTransmission"/> provided by the
        ///     <see cref="EventPipelineConfiguratorExtensions.ThenIsPublishedToGlobalSubscriptions{TSource,TEventArgs}(Config.EventPipelineConfigurator{TSource,TEventArgs},System.Func{ConfigureTransmission, IPublishTransmissionConfiguration})"/>
        ///     method.
        /// </param>
        /// <returns>
        ///     The <see cref="IPublishTransmissionConfiguration"/> for the
        ///     <see cref="EventPipelineConfiguratorExtensions.ThenIsPublishedToGlobalSubscriptions{TSource,TEventArgs}(Config.EventPipelineConfigurator{TSource,TEventArgs},System.Func{ConfigureTransmission, IPublishTransmissionConfiguration})"/>
        ///     method.
        /// </returns>
        public static IPublishTransmissionConfiguration WithAzureTopic(
            this IConfigureTransmission configureTransmission
        )
        {
            return configureTransmission.With<AzureTopicEventSender>();
        }
    }
}
