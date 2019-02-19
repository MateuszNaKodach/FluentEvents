using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public static class EventConfiguratorExtensions
    {
        /// <summary>
        /// This method creates a pipeline for the current event.
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventConfigurator">
        ///     The event configurator provided by
        ///     the <see cref="PipelinesBuilder.Event{TSource, TEventArgs}(string)"/> method.
        /// </param>
        /// <returns>An <see cref="EventPipelineConfigurator{TSource,TEventArgs}"/> to configure the modules of the pipeline.</returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> IsForwardedToPipeline<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            var pipeline = new Pipeline(eventConfigurator.Get<EventsContext>().Get<IServiceProvider>());

            eventConfigurator.Get<SourceModelEventField>().AddPipeline(pipeline);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                eventConfigurator
            );
        }
    }
}
