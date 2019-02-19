using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Projections
{
    public static class EventPipelineConfiguratorExtensions
    {
        /// <summary>
        ///     Adds a module to the current pipeline that replaces the event sender and the event args with a projection.
        /// </summary>
        /// <remarks>
        ///     Projections are useful when an event needs to be serialized in order to reduce
        ///     the size and the complexity of the serialization output.
        /// </remarks>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TToSource">The type of the projected event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <typeparam name="TToEventArgs">The type of the projected event args.</typeparam>
        /// <param name="eventPipelineConfigurator">
        ///     The <see cref="EventPipelineConfigurator{TSource, TEventArgs}"/> for the pipeline being configured.
        /// </param>
        /// <param name="senderConverter">
        ///     A <see cref="Func{TSource, TToSource}"/> that takes the event source as input and returns a new object.
        /// </param>
        /// <param name="eventArgsConverter">
        ///     A <see cref="Func{TEventArgs, TToEventArgs}"/> that takes the event args as input and returns a new object.
        /// </param>
        /// <returns>
        ///     A new <see cref="EventPipelineConfigurator{TToSource, TToEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TToSource, TToEventArgs> ThenIsProjected<TSource, TToSource, TEventArgs, TToEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TToSource> senderConverter,
            Func<TEventArgs, TToEventArgs> eventArgsConverter
        )
            where TSource : class
            where TEventArgs : class
            where TToSource : class
            where TToEventArgs : class
        {
            var serviceProvider = eventPipelineConfigurator.Get<EventsContext>().Get<IServiceProvider>();
            var sourceModelsService = serviceProvider.GetRequiredService<ISourceModelsService>();

            var projectedSourceModel = sourceModelsService.GetOrCreateSourceModel(
                typeof(TToSource)
            );

            var projectedEventField = projectedSourceModel.GetOrCreateEventField(
                eventPipelineConfigurator.Get<SourceModelEventField>().Name
            );

            var projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                new EventsSenderProjection<TSource, TToSource>(senderConverter),
                new EventArgsProjection<TEventArgs, TToEventArgs>(eventArgsConverter)
            );

            eventPipelineConfigurator
                .Get<Pipeline>()
                .AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                    projectionPipelineModuleConfig
                );

            return new EventPipelineConfigurator<TToSource, TToEventArgs>(
                projectedSourceModel,
                projectedEventField,
                eventPipelineConfigurator.Get<EventsContext>(),
                eventPipelineConfigurator.Get<Pipeline>()
            );
        }
    }
}
