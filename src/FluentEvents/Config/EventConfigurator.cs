using FluentEvents.Model;

namespace FluentEvents.Config
{
    public class EventConfigurator<TSource, TEventArgs> : IEventConfigurator
        where TSource : class 
        where TEventArgs : class 
    {
        SourceModel IEventConfigurator.SourceModel => m_SourceModel;
        SourceModelEventField IEventConfigurator.SourceModelEventField => m_SourceModelEventField;
        EventsContext IEventConfigurator.EventsContext => m_EventsContext;

        private readonly SourceModel m_SourceModel;
        private readonly SourceModelEventField m_SourceModelEventField;
        private readonly EventsContext m_EventsContext;

        public EventConfigurator(
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField,
            EventsContext eventsContext
        )
        {
            m_SourceModel = sourceModel;
            m_SourceModelEventField = sourceModelEventField;
            m_EventsContext = eventsContext;
        }

    }
}