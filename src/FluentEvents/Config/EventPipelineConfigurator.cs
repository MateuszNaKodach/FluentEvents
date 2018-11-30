using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public class EventPipelineConfigurator<TSource, TEventArgs> : 
        EventConfigurator<TSource, TEventArgs>, 
        IEventPipelineConfigurator 
        where TSource : class
        where TEventArgs : class
    {
        Pipeline IEventPipelineConfigurator.Pipeline => m_Pipeline;
        private readonly Pipeline m_Pipeline;

        public EventPipelineConfigurator(
            Pipeline pipeline,
            IEventConfigurator eventConfigurator
        ) : base(
            eventConfigurator.SourceModel,
            eventConfigurator.SourceModelEventField,
            eventConfigurator.EventsContext
        )
        {
            m_Pipeline = pipeline;
        }

        public EventPipelineConfigurator(
            IEventPipelineConfigurator eventPipelineConfigurator
        ) : base(
            eventPipelineConfigurator.SourceModel,
            eventPipelineConfigurator.SourceModelEventField,
            eventPipelineConfigurator.EventsContext
        )
        {
            m_Pipeline = eventPipelineConfigurator.Pipeline;
        }

        public EventPipelineConfigurator(
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField,
            IEventPipelineConfigurator eventPipelineConfigurator
        ) : base(
            sourceModel,
            sourceModelEventField,
            eventPipelineConfigurator.EventsContext
        )
        {
            m_Pipeline = eventPipelineConfigurator.Pipeline;
        }
    }
}