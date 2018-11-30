using System;
using FluentEvents.Config;

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

        internal static EventPipelineConfigurator<TSource, TEventArgs> IsQueuedToInternal<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator,
            string queueName
        )
            where TSource : class
            where TEventArgs : class
        {
            var eventPipelineConfig = ((IEventConfigurator)eventConfigurator)
                .SourceModelEventField
                .AddEventPipelineConfig(queueName);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                eventPipelineConfig,
                eventConfigurator
            );
        }
    }
}
