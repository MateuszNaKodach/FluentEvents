using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public class EventPipelineConfigurator<TSource, TEventArgs> : IEventPipelineConfigurator 
        where TSource : class
        where TEventArgs : class
    {
        SourceModel IEventPipelineConfigurator.SourceModel => m_SourceModel;
        SourceModelEventField IEventPipelineConfigurator.SourceModelEventField => m_SourceModelEventField;
        EventsContext IEventPipelineConfigurator.EventsContext => m_EventsContext;
        Pipeline IEventPipelineConfigurator.Pipeline => m_Pipeline;

        private readonly SourceModel m_SourceModel;
        private readonly SourceModelEventField m_SourceModelEventField;
        private readonly EventsContext m_EventsContext;
        private readonly Pipeline m_Pipeline;

        public EventPipelineConfigurator(
            Pipeline pipeline,
            IEventConfigurator eventConfigurator
        )
        {
            m_SourceModel = eventConfigurator.SourceModel;
            m_SourceModelEventField = eventConfigurator.SourceModelEventField;
            m_EventsContext = eventConfigurator.EventsContext;
            m_Pipeline = pipeline;
        }
        
        public EventPipelineConfigurator(
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField,
            IEventPipelineConfigurator eventPipelineConfigurator
        )
        {
            m_SourceModel = sourceModel;
            m_SourceModelEventField = sourceModelEventField;
            m_EventsContext = eventPipelineConfigurator.EventsContext;
            m_Pipeline = eventPipelineConfigurator.Pipeline;
        }
    }
}