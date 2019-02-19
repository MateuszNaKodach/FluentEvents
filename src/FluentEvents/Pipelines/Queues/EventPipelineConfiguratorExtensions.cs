using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Queues
{
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds module to the current pipeline that queues the event in the default queue
        ///     and pauses the execution of the current pipeline until the event is dequeued.
        /// </summary>
        /// <remarks>The name of the default queue is "Default".</remarks>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsQueuedToDefaultQueue<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator
        )
            where TSource : class
            where TEventArgs : class
            => eventPipelineConfigurator.IsQueuedToInternal("Default");

        /// <summary>
        ///     Adds module to the current pipeline that queues the event in a queue
        ///     and pauses the execution of the current pipeline until the event is dequeued.
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsQueuedTo<TSource, TEventArgs>(
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
            if (queueName != null)
            {
                var eventsQueueNamesService = eventPipelineConfigurator.Get<EventsContext>().Get<IServiceProvider>()
                    .GetRequiredService<IEventsQueueNamesService>();

                eventsQueueNamesService.RegisterQueueNameIfNotExists(queueName);
            }

            eventPipelineConfigurator
                .Get<Pipeline>()
                .AddModule<EnqueuePipelineModule, EnqueuePipelineModuleConfig>(
                    new EnqueuePipelineModuleConfig
                    {
                        QueueName = queueName
                    }
                );

            return eventPipelineConfigurator;
        }
    }
}
