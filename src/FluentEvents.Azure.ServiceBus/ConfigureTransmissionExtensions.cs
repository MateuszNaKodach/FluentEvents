using System;
using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Configuration;
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
        ///     <see cref="EventPipelineConfigurationExtensions.ThenIsPublishedToGlobalSubscriptions{TEvent}(EventPipelineConfiguration{TEvent},Func{ConfigureTransmission, IPublishTransmissionConfiguration})"/>
        ///     method.
        /// </param>
        /// <returns>
        ///     The <see cref="IPublishTransmissionConfiguration"/> for the
        ///     <see cref="EventPipelineConfigurationExtensions.ThenIsPublishedToGlobalSubscriptions{TEvent}(EventPipelineConfiguration{TEvent},Func{ConfigureTransmission, IPublishTransmissionConfiguration})"/>
        ///     method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="configureTransmission"/> is <see langword="null"/>.
        /// </exception>
        public static IPublishTransmissionConfiguration WithAzureTopic(
            this IConfigureTransmission configureTransmission
        )
        {
            if (configureTransmission == null) throw new ArgumentNullException(nameof(configureTransmission));

            return configureTransmission.With<AzureTopicEventSender>();
        }
    }
}
