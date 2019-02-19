using FluentEvents.Infrastructure;
using FluentEvents.Model;

namespace FluentEvents.Config
{
    public class EventConfigurator<TSource, TEventArgs> 
        : IInfrastructure<SourceModel>,
            IInfrastructure<SourceModelEventField>,
            IInfrastructure<EventsContext>
        where TSource : class 
        where TEventArgs : class 
    {
        SourceModel IInfrastructure<SourceModel>.Instance => m_SourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => m_SourceModelEventField;
        EventsContext IInfrastructure<EventsContext>.Instance => m_EventsContext;

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