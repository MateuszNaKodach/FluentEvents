using System;
using FluentEvents.Config;

namespace FluentEvents.Pipelines.Publication
{
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        /// Publishes the event to all the subscriptions in scope locally.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">The configuration object for the specified event.</param>
        /// <returns>The configuration object supplied in <param name="eventPipelineConfigurator"></param>.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToScopedSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            ((IEventPipelineConfigurator)eventPipelineConfigurator).Pipeline.AddModule<ScopedPublishPipelineModule>(
                new ScopedPublishPipelineModuleConfig()
            );
            return eventPipelineConfigurator;
        }

        /// <summary>
        /// Publishes the event to all the global subscriptions using a transmission method
        /// configurable with the <param name="configurePublishTransmission" /> parameter.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">The configuration object for the specified event.</param>
        /// <param name="configurePublishTransmission">A delegate for configuring how the event is transmitted.</param>
        /// <returns>The configuration object supplied in <param name="eventPipelineConfigurator" />.</returns>
        /// <remarks>This method can be used to configure a publication to multiple application instances with this <see cref="EventsContext"/></remarks>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator, 
            Func<GlobalPublishingOptionsFactory, IPublishTransmissionConfiguration> configurePublishTransmission
        )
            where TSource : class
            where TEventArgs : class
        {
            var globalPublishingOptionsFactory = new GlobalPublishingOptionsFactory();
            var senderTypeConfiguration = configurePublishTransmission(globalPublishingOptionsFactory);
            var moduleConfig = new GlobalPublishPipelineModuleConfig
            {
                SenderType = senderTypeConfiguration.SenderType
            };

            ((IEventPipelineConfigurator) eventPipelineConfigurator).Pipeline
                .AddModule<GlobalPublishPipelineModule>(
                    moduleConfig
                );
            return eventPipelineConfigurator;
        }

        /// <summary>
        /// Publishes the event to all the global subscriptions locally.
        /// </summary>
        /// <typeparam name="TSource">The type that publishes the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">The configuration object for the specified event.</param>
        /// <returns>The configuration object supplied in <param name="eventPipelineConfigurator" />.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(x => x.Locally());
    }
}
