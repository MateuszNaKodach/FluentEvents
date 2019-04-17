using System;
using System.Linq;
using FluentEvents.Model;
using FluentEvents.Utils;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface to select an event and configure it fluently.
    /// </summary>
    public class PipelinesBuilder
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ISourceModelsService m_SourceModelsService;
        private readonly IEventSelectionService m_EventSelectionService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PipelinesBuilder(
            IServiceProvider serviceProvider,
            ISourceModelsService sourceModelsService,
            IEventSelectionService eventSelectionService
        )
        {
            m_ServiceProvider = serviceProvider;
            m_SourceModelsService = sourceModelsService;
            m_EventSelectionService = eventSelectionService;
        }

        /// <summary>
        ///     Registers a source and an event as part of the model and returns an object that can be used to
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
            if (eventFieldName == null) throw new ArgumentNullException(nameof(eventFieldName));

            var sourceModel = m_SourceModelsService.GetOrCreateSourceModel(typeof(TSource));

            return Event<TSource, TEventArgs>(sourceModel, eventFieldName);
        }
        
        /// <summary>
        ///     Registers a source and an event as part of the model and returns an object that can be used to
        ///     configure how the event is handled. This method can be called multiple times for the same event to
        ///     configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="eventSelectionAction">
        ///     This parameter accepts an <see cref="Action{TSource, dynamic}"/> that subscribes the dynamic object
        ///     supplied in the second <see cref="Action{TSource, dynamic}"/> parameter to the event being selected.
        ///     Example usage: <code>(source, eventHandler) =&gt; source.MyEvent += eventHandler</code>
        /// </param>
        /// <example>
        ///     Event&lt;MySource, MyEventArgs&gt;((source, eventHandler) =&gt; source.MyEvent += eventHandler)
        /// </example>
        /// <returns>The configuration object for the specified event.</returns>
        public EventConfigurator<TSource, TEventArgs> Event<TSource, TEventArgs>(
            Action<TSource, dynamic> eventSelectionAction
        )
            where TSource : class
            where TEventArgs : class
        {
            if (eventSelectionAction == null) throw new ArgumentNullException(nameof(eventSelectionAction));

            var sourceModel = m_SourceModelsService.GetOrCreateSourceModel(typeof(TSource));
            var eventFieldNames = m_EventSelectionService.GetSelectedEvent(sourceModel, eventSelectionAction);

            if (eventFieldNames.Count() > 1)
                throw new MoreThanOneEventSelectedException();

            if (!eventFieldNames.Any())
                throw new NoEventsSelectedException();

            var eventFieldName = eventFieldNames.FirstOrDefault();

            return Event<TSource, TEventArgs>(sourceModel, eventFieldName);
        }

        private EventConfigurator<TSource, TEventArgs> Event<TSource, TEventArgs>(
            SourceModel sourceModel,
            string eventFieldName
        )
            where TSource : class where TEventArgs : class
        {
            var eventField = sourceModel.GetOrCreateEventField(eventFieldName);

            if (eventField.EventArgsType != typeof(TEventArgs))
                throw new EventArgsTypeMismatchException();

            return new EventConfigurator<TSource, TEventArgs>(m_ServiceProvider, sourceModel, eventField);
        }
    }
}
