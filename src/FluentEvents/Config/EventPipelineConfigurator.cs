using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event pipeline.
    /// </summary>
    public class EventPipelineConfigurator<TSource, TEventArgs>
        : IInfrastructure<SourceModel>,
            IInfrastructure<SourceModelEventField>,
            IInfrastructure<EventsContext>,
            IInfrastructure<Pipeline>
        where TSource : class
        where TEventArgs : class
    {
        SourceModel IInfrastructure<SourceModel>.Instance => m_SourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => m_SourceModelEventField;
        EventsContext IInfrastructure<EventsContext>.Instance => m_EventsContext;
        Pipeline IInfrastructure<Pipeline>.Instance => m_Pipeline;

        private readonly SourceModel m_SourceModel;
        private readonly SourceModelEventField m_SourceModelEventField;
        private readonly EventsContext m_EventsContext;
        private readonly Pipeline m_Pipeline;

        /// <summary>
        ///     Creates an instance by taking dependencies from an <see cref="EventConfigurator{TSource,TEventArgs}"/>
        /// </summary>
        /// <param name="pipeline">The pipeline to configure.</param>
        /// <param name="eventConfigurator">The event configurator.</param>
        public EventPipelineConfigurator(
            Pipeline pipeline,
            EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
        {
            m_SourceModel = eventConfigurator.Get<SourceModel>();
            m_SourceModelEventField = eventConfigurator.Get<SourceModelEventField>();
            m_EventsContext = eventConfigurator.Get<EventsContext>();
            m_Pipeline = pipeline;
        }

        /// <summary>
        ///     Allows to create an instance without passing an <see cref="EventConfigurator{TSource,TEventArgs}"/>
        /// </summary>
        /// <param name="sourceModel">The source model.</param>
        /// <param name="sourceModelEventField">The source model event field.</param>
        /// <param name="eventsContext">The events context.</param>
        /// <param name="pipeline">The pipeline to configure.</param>
        public EventPipelineConfigurator(
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField,
            EventsContext eventsContext,
            Pipeline pipeline
        )
        {
            m_SourceModel = sourceModel;
            m_SourceModelEventField = sourceModelEventField;
            m_EventsContext = eventsContext;
            m_Pipeline = pipeline;
        }
    }
}