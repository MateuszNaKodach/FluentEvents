using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="eventFieldName">The name of the event field on the projection object (The default is the name of the event being configured).</param>
        /// <returns>
        ///     A new <see cref="EventPipelineConfigurator{TToSource, TToEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TToSource, TToEventArgs> ThenIsProjected<TSource, TToSource, TEventArgs, TToEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TToSource> senderConverter,
            Func<TEventArgs, TToEventArgs> eventArgsConverter,
            string eventFieldName = null
        )
            where TSource : class
            where TEventArgs : class
            where TToSource : class
            where TToEventArgs : class
        {
            if (senderConverter == null) throw new ArgumentNullException(nameof(senderConverter));
            if (eventArgsConverter == null) throw new ArgumentNullException(nameof(eventArgsConverter));
            
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModelsService = serviceProvider.GetRequiredService<ISourceModelsService>();

            var projectedSourceModel = sourceModelsService.GetOrCreateSourceModel(
                typeof(TToSource)
            );

            return ThenIsProjected(
                eventPipelineConfigurator,
                senderConverter,
                eventArgsConverter,
                projectedSourceModel,
                eventFieldName
            );
        }

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
        /// <param name="eventSelectionAction">
        ///     This parameter accepts an <see cref="Action{TSource, dynamic}"/> that subscribes the dynamic object
        ///     supplied in the second <see cref="Action{TSource, dynamic}"/> parameter to the event being selected.
        ///     Example usage: <code>(source, eventHandler) =&gt; source.MyEvent += eventHandler</code>
        /// </param>
        /// <example>
        ///     ThenIsProjected(
        ///         source =&gt; new SourceProjection(),
        ///         eventArgs =&gt; new EventArgsProjection(),
        ///         (source, eventHandler) =&gt; source.MyEvent += eventHandler
        ///     )
        /// </example>
        /// <returns>
        ///     A new <see cref="EventPipelineConfigurator{TToSource, TToEventArgs}"/> instance so that multiple calls can be chained.
        /// </returns>
        public static EventPipelineConfigurator<TToSource, TToEventArgs> ThenIsProjected<TSource, TToSource, TEventArgs, TToEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TToSource> senderConverter,
            Func<TEventArgs, TToEventArgs> eventArgsConverter,
            Action<TToSource, dynamic> eventSelectionAction
        )
            where TSource : class
            where TEventArgs : class
            where TToSource : class
            where TToEventArgs : class
        {
            if (senderConverter == null) throw new ArgumentNullException(nameof(senderConverter));
            if (eventArgsConverter == null) throw new ArgumentNullException(nameof(eventArgsConverter));

            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModelsService = serviceProvider.GetRequiredService<ISourceModelsService>();
            var eventSelectionService = serviceProvider.GetRequiredService<IEventSelectionService>();

            var projectedSourceModel = sourceModelsService.GetOrCreateSourceModel(typeof(TToSource));
            var eventFieldName = eventSelectionService.GetSingleSelectedEventName(projectedSourceModel, eventSelectionAction);

            return ThenIsProjected(
                eventPipelineConfigurator, 
                senderConverter,
                eventArgsConverter, 
                projectedSourceModel,
                eventFieldName
            );
        }

        private static EventPipelineConfigurator<TToSource, TToEventArgs> ThenIsProjected<TSource, TToSource, TEventArgs, TToEventArgs>(
            EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TToSource> senderConverter, Func<TEventArgs, TToEventArgs> eventArgsConverter,
            SourceModel projectedSourceModel,
            string eventFieldName
        )
            where TSource : class
            where TEventArgs : class
            where TToSource : class
            where TToEventArgs : class
        {
            var projectedEventField = projectedSourceModel.GetOrCreateEventField(
                eventFieldName ?? eventPipelineConfigurator.Get<SourceModelEventField>().Name
            );

            var projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                new EventSenderProjection<TSource, TToSource>(senderConverter),
                new EventArgsProjection<TEventArgs, TToEventArgs>(eventArgsConverter),
                projectedEventField.Name
            );

            eventPipelineConfigurator
                .Get<IPipeline>()
                .AddModule<ProjectionPipelineModule, ProjectionPipelineModuleConfig>(
                    projectionPipelineModuleConfig
                );

            return new EventPipelineConfigurator<TToSource, TToEventArgs>(
                projectedSourceModel,
                projectedEventField,
                eventPipelineConfigurator.Get<IServiceProvider>(),
                eventPipelineConfigurator.Get<IPipeline>()
            );
        }
    }
}
