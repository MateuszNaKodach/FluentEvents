using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Queues
{
    public static class EventConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TSource, TEventArgs> IsNotQueued<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventConfigurator.IsQueuedToInternal(null);
        
        public static EventPipelineConfigurator<TSource, TEventArgs> IsQueuedToDefaultQueue<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventConfigurator.IsQueuedToInternal("Default");
        
        public static EventPipelineConfigurator<TSource, TEventArgs> IsQueuedTo<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator,
            string queueName
        )
            where TSource : class
            where TEventArgs : class
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            return eventConfigurator.IsQueuedToInternal(queueName);
        }

        private static EventPipelineConfigurator<TSource, TEventArgs> IsQueuedToInternal<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator,
            string queueName
        )
            where TSource : class
            where TEventArgs : class
        {
            var configurator = (IEventConfigurator) eventConfigurator;

            var pipeline = new Pipeline(
                queueName,
                configurator.SourceModel.EventsContext,
                configurator.EventsContext.Get<IServiceProvider>()
            );

            configurator
                .SourceModelEventField
                .AddEventPipelineConfig(pipeline);

            var eventsQueueNamesService = configurator.EventsContext.Get<IServiceProvider>()
                .GetRequiredService<IEventsQueueNamesService>();

            eventsQueueNamesService.RegisterQueueNameIfNotExists(queueName);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                eventConfigurator
            );
        }
    }
}
