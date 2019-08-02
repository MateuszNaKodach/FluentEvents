using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;

namespace FluentEvents.Pipelines.Projections
{
    /// <summary>
    ///     Extension methods for adding a projection to a pipeline.
    /// </summary>
    public static class EventPipelineConfigurationExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that replaces event with a projection.
        /// </summary>
        /// <remarks>
        ///     Projections are useful when an event needs to be serialized in order to reduce
        ///     the size and the complexity of the serialization output.
        /// </remarks>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <typeparam name="TToEvent">The type of the projected event.</typeparam>
        /// <param name="eventPipelineConfiguration">
        ///     The <see cref="EventPipelineConfiguration{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="eventConverter">
        ///     A <see cref="Func{TEvent, TToEvent}"/> that takes the event as input and returns a new object.
        /// </param>
        /// <returns>
        ///     A new <see cref="EventPipelineConfiguration{TEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="eventPipelineConfiguration"/> and/or <paramref name="eventConverter"/> are <see langword="null"/>.
        /// </exception>
        public static EventPipelineConfiguration<TToEvent> ThenIsProjected<TEvent, TToEvent>(
            this EventPipelineConfiguration<TEvent> eventPipelineConfiguration,
            Func<TEvent, TToEvent> eventConverter
        )
            where TEvent : class
            where TToEvent : class
        {
            if (eventPipelineConfiguration == null) throw new ArgumentNullException(nameof(eventPipelineConfiguration));
            if (eventConverter == null) throw new ArgumentNullException(nameof(eventConverter));
            
            var projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                new EventProjection<TEvent, TToEvent>(eventConverter)
            );

            eventPipelineConfiguration
                .Get<IPipeline>()
                .AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                    projectionPipelineModuleConfig
                );

            return new EventPipelineConfiguration<TToEvent>(
                eventPipelineConfiguration.Get<IServiceProvider>(),
                eventPipelineConfiguration.Get<IPipeline>()
            );
        }
    }
}
