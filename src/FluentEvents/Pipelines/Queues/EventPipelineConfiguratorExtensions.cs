using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Queues
{
    /// <summary>
    ///     Extension methods for adding a pipeline module that queues the events.
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds module to the current pipeline that queues the event in a queue
        ///     and pauses the execution of the current pipeline until the event is dequeued.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TEvent> ThenIsQueuedTo<TEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            string queueName
        )
            where TEvent : class
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            var eventsQueueNamesService = eventPipelineConfigurator
                .Get<IServiceProvider>()
                .GetRequiredService<IEventsQueueNamesService>();

            eventsQueueNamesService.RegisterQueueNameIfNotExists(queueName);

            eventPipelineConfigurator
                .Get<IPipeline>()
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
