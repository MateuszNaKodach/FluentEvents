using System;
using System.Linq;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     Extension methods for adding a publishing module to the pipeline.
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the subscriptions in scope locally.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TEvent> ThenIsPublishedToScopedSubscriptions<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator
        )
            where TEvent : class
        {
            eventPipelineConfigurator
                .Get<IPipeline>()
                .AddModule<ScopedPublishPipelineModule, ScopedPublishPipelineModuleConfig>(
                    new ScopedPublishPipelineModuleConfig()
                );

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the global subscriptions using a transmission method
        ///     configurable with the configurePublishTransmission parameter.
        /// </summary>
        /// <remarks>
        ///     This method can be used to configure a publication to multiple application instances with this <see cref="EventsContext"/>
        /// </remarks>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="configurePublishTransmission">A delegate for configuring how the event is transmitted.</param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TEvent> ThenIsPublishedToGlobalSubscriptions<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator, 
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
                var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
                var eventSenderExists = serviceProvider
                    .GetServices<IEventSender>()
                    .Any(x => x.GetType() == moduleConfig.SenderType);

                if (!eventSenderExists)
                    throw new EventTransmissionPluginIsNotConfiguredException();
            }

            eventPipelineConfigurator
                .Get<IPipeline>()
                .AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                    moduleConfig
                );

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the global subscriptions locally.
        /// </summary>
        /// <typeparam name="TEvent">The type that publishes the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TEvent> ThenIsPublishedToGlobalSubscriptions<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator
        )
            where TEvent : class

            => eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(x => x.Locally());
    }
}
