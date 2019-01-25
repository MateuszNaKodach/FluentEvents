using System;
using FluentEvents.Model;

namespace FluentEvents.Config
{
    public class PipelinesBuilder : BuilderBase
    {
        private readonly ISourceModelsService m_SourceModelsService;

        public PipelinesBuilder(
            EventsContext eventsContext,
            IServiceProvider serviceProvider,
            ISourceModelsService sourceModelsService
        )
            : base(eventsContext, serviceProvider)
        {
            m_SourceModelsService = sourceModelsService;
        }

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

        public EventConfigurator<TSource, TEventArgs> Event<TSource, TEventArgs>(
            Action<TSource> eventSelector
        )
            where TSource : class
            where TEventArgs : class
        {
            throw new NotImplementedException();
        }
    }
}
