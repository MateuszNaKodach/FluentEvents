using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;

namespace FluentEvents.Pipelines.Filters
{
    /// <summary>
    ///     Extension methods for adding an events filter to a pipeline.
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds an event filtering module to the current pipeline.
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="filter">
        ///     A <see cref="Func{TSource, TEventArgs, TResult}"/> that takes the event sender and the event args
        ///     as input and returns false if the event should be filtered
        ///     (When an event is filtered any module configured after the filter won't be invoked).
        /// </param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsFiltered<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TEventArgs, bool> filter
        )
            where TSource : class 
            where TEventArgs : class
        {
            eventPipelineConfigurator.Get<Pipeline>()
                .AddModule<FilterPipelineModule, FilterPipelineModuleConfig>(
                    new FilterPipelineModuleConfig((sender, args) => filter((TSource) sender, (TEventArgs) args))
                );

            return eventPipelineConfigurator;
        }
    }
}
