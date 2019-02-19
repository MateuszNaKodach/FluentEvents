using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;

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
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToScopedSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            eventPipelineConfigurator
                .Get<Pipeline>()
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
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="configurePublishTransmission">A delegate for configuring how the event is transmitted.</param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator, 
            Func<ConfigureTransmission, IPublishTransmissionConfiguration> configurePublishTransmission
        )
            where TSource : class
            where TEventArgs : class
        {
            var globalPublishingOptionsFactory = new ConfigureTransmission();
            var senderTypeConfiguration = configurePublishTransmission(globalPublishingOptionsFactory);
            var moduleConfig = new GlobalPublishPipelineModuleConfig
            {
                SenderType = senderTypeConfiguration.SenderType
            };

            eventPipelineConfigurator
                .Get<Pipeline>()
                .AddModule<GlobalPublishPipelineModule, GlobalPublishPipelineModuleConfig>(
                    moduleConfig
                );

            return eventPipelineConfigurator;
        }

        /// <summary>
        ///     Adds a module to the current pipeline that publishes the event to all the global subscriptions locally.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <returns>The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(x => x.Locally());
    }
}
