using System;
using System.Linq;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     Extension methods for adding a publishing module to the pipeline.
    /// </summary>
    public static class EventPipelineConfigurationExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the subscriptions in scope locally.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="eventPipelineConfiguration"/> is <see langword="null"/>.
        /// </exception>
        public static EventPipelineConfiguration<TEvent> ThenIsPublishedToScopedSubscriptions<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration
        )
            where TEvent : class
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));

            eventPipelineConfiguration
                .Get<IPipeline>()
                .AddModule<ScopedPublishPipelineModule, ScopedPublishPipelineModuleConfig>(
                    new ScopedPublishPipelineModuleConfig()
                );

            return eventPipelineConfiguration;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the global subscriptions using a transmission method
        ///     configurable with the configurePublishTransmission parameter.
        /// </summary>
        /// <remarks>
        ///     This method can be used to configure a publication to multiple application instances with this <see cref="EventsContext"/>
        /// </remarks>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="configurePublishTransmission">A delegate for configuring how the event is transmitted.</param>
        /// <returns>The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="configurePublishTransmission"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="EventTransmissionPluginIsNotConfiguredException">
        ///     A transmission method has been specified but it's plugin wasn't configured in the <see cref="EventsContextOptions"/>.
        /// </exception>
        public static EventPipelineConfiguration<TEvent> ThenIsPublishedToGlobalSubscriptions<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration, 
            Func<ConfigureTransmission, IPublishTransmissionConfiguration> configurePublishTransmission
        )
            where TEvent : class
        {
            if (configurePublishTransmission == null)
                throw new ArgumentNullException(nameof(configurePublishTransmission));

            var globalPublishingOptionsFactory = new ConfigureTransmission();
            var senderTypeConfiguration = configurePublishTransmission(globalPublishingOptionsFactory);
            var moduleConfig = new GlobalPublishPipelineModuleConfig
            {
                SenderType = senderTypeConfiguration.SenderType
            };

            if (moduleConfig.SenderType != null)
            {
                var serviceProvider = eventPipelineConfiguration.Get<IServiceProvider>();
                var eventSenderExists = serviceProvider
                    .GetServices<IEventSender>()
                    .Any(x => x.GetType() == moduleConfig.SenderType);

                if (!eventSenderExists)
                    throw new EventTransmissionPluginIsNotConfiguredException();
            }

            eventPipelineConfiguration
                .Get<IPipeline>()
                .AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                    moduleConfig
                );

            return eventPipelineConfiguration;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the global subscriptions locally.
        /// </summary>
        /// <typeparam name="TEvent">The type that publishes the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfiguration<TEvent> ThenIsPublishedToGlobalSubscriptions<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration
        )
            where TEvent : class

            => eventPipelineConfiguration.ThenIsPublishedToGlobalSubscriptions(x => ConfigureTransmission.Locally());
    }
}
