using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;

namespace FluentEvents.Pipelines.Projections
{
    /// <summary>
    ///     Extension methods for adding a projection to a pipeline.
    /// </summary>
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that replaces the event sender and the event args with a projection.
        /// </summary>
        /// <remarks>
        ///     Projections are useful when an event needs to be serialized in order to reduce
        ///     the size and the complexity of the serialization output.
        /// </remarks>
        /// <typeparam name="TEvent">The type of the event args.</typeparam>
        /// <typeparam name="TToEvent">The type of the projected event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TEvent}"/> for the pipeline being configured.
        /// </param>
        /// <param name="eventConverter">
        ///     A <see cref="Func{TEvent, TToEvent}"/> that takes the event args as input and returns a new object.
        /// </param>
        /// <returns>
        ///     A new <see cref="EventPipelineConfigurator{TToEvent}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TToEvent> ThenIsProjected<TEvent, TToEvent>(
            this EventPipelineConfigurator<TEvent> eventPipelineConfigurator,
            Func<TEvent, TToEvent> eventConverter
        )
            where TEvent : class
            where TToEvent : class
        {
            if (eventConverter == null) throw new ArgumentNullException(nameof(eventConverter));
            
            var projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                new EventProjection<TEvent, TToEvent>(eventConverter)
            );

            eventPipelineConfigurator
                .Get<IPipeline>()
                .AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                    projectionPipelineModuleConfig
                );

            return new EventPipelineConfigurator<TToEvent>(
                eventPipelineConfigurator.Get<IServiceProvider>(),
                eventPipelineConfigurator.Get<IPipeline>()
            );
        }
    }
}
