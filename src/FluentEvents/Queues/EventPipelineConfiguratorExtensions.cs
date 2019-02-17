using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Queues
{
    public static class EventPipelineConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsEnqueuedToDefaultQueue<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventPipelineConfigurator.IsQueuedToInternal("Default");
        
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsEnqueuedTo<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            string queueName
        )
            where TSource : class
            where TEventArgs : class
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            return eventPipelineConfigurator.IsQueuedToInternal(queueName);
        }

        private static EventPipelineConfigurator<TSource, TEventArgs> IsQueuedToInternal<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            string queueName
        )
            where TSource : class
            where TEventArgs : class
        {
            var configurator = (IEventPipelineConfigurator) eventPipelineConfigurator;

            if (queueName != null)
            {
                var eventsQueueNamesService = configurator.EventsContext.Get<IServiceProvider>()
                    .GetRequiredService<IEventsQueueNamesService>();

                eventsQueueNamesService.RegisterQueueNameIfNotExists(queueName);
            }

            configurator.Pipeline.AddModule<EnqueuePipelineModule, EnqueuePipelineModuleConfig>(
                new EnqueuePipelineModuleConfig
                {
                    QueueName = queueName
                }
            );

            return eventPipelineConfigurator;
        }
    }
}
