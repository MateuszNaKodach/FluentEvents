using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;

namespace FluentEvents.Pipelines.Filters
{
    /// <summary>
    ///     Extension methods for adding an events filter to a pipeline.
    /// </summary>
    public static class EventPipelineConfigurationExtensions
    {
        /// <summary>
        ///     Adds an event filtering module to the current pipeline.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="filter">
        ///     A <see cref="Func{TEvent, TResult}"/> that takes the event
        ///     as input and returns false if it should be filtered
        ///     (When an event is filtered any module configured after the filter won't be invoked).
        /// </param>
        /// <returns>
        ///     The same <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="eventPipelineConfiguration"/> and/or <paramref name="filter"/> are <see langword="null"/>.
        /// </exception>
        public static EventPipelineConfiguration<TEvent> ThenIsFiltered<TEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            Func<TEvent, bool> filter
        )
            where TEvent : class 
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            eventPipelineConfiguration.Get<IPipeline>()
                .AddModule<FilterPipelineModule, FilterPipelineModuleConfig>(
                    new FilterPipelineModuleConfig(pipedEvent => filter((TEvent) pipedEvent))
                );

            return eventPipelineConfiguration;
        }
    }
}
