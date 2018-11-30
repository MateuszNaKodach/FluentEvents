using System;
using FluentEvents.Config;

namespace FluentEvents.Pipelines.Publication
{
    public static class EventPipelineConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToScopedSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            ((IEventPipelineConfigurator)eventPipelineConfigurator).Pipeline.AddModuleConfig(
                new ScopedPublishPipelineModuleConfig()
            );
            return eventPipelineConfigurator;
        }

        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator, 
            Func<GlobalPublishingOptionsFactory, ISenderTypeConfiguration> optionsConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            var factory = new GlobalPublishingOptionsFactory();
            var senderTypeConfiguration = optionsConfigurator(factory);
            var moduleConfig = new GlobalPublishPipelineModuleConfig
            {
                SenderType = senderTypeConfiguration.SenderType
            };

            ((IEventPipelineConfigurator)eventPipelineConfigurator).Pipeline.AddModuleConfig(moduleConfig);
            return eventPipelineConfigurator;
        }

        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsPublishedToGlobalSubscriptions<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventPipelineConfigurator.ThenIsPublishedToGlobalSubscriptions(x => x.Locally());
    }
}
