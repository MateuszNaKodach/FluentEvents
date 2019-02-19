using System;
using FluentEvents.Model;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface to select an event and configure it fluently.
    /// </summary>
    public class PipelinesBuilder : BuilderBase
    {
        private readonly ISourceModelsService m_SourceModelsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PipelinesBuilder(
            EventsContext eventsContext,
            IServiceProvider serviceProvider,
            ISourceModelsService sourceModelsService
        )
            : base(eventsContext, serviceProvider)
        {
            m_SourceModelsService = sourceModelsService;
        }

        /// <summary>
        ///     Registers an entity and an event as part of the model and returns an object that can be used to
        ///     configure how the event is handled. This method can be called multiple times for the same event to
        ///     configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventFieldName">The name of the event field.</param>
        /// <returns>The configuration object for the specified event.</returns>
        public EventConfigurator<TSource, TEventArgs> Event<TSource, TEventArgs>(
            string eventFieldName
        )
            where TSource : class
            where TEventArgs : class
        {
            var sourceModel = m_SourceModelsService.GetOrCreateSourceModel(typeof(TSource));
            var eventField = sourceModel.GetOrCreateEventField(eventFieldName);

            return new EventConfigurator<TSource, TEventArgs>(sourceModel, eventField, EventsContext);
        }
    }
}
