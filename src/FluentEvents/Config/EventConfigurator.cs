using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

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

        /// <summary>
        ///     This method creates a pipeline for the current event.
        /// </summary>
        /// <returns>An <see cref="EventPipelineConfigurator{TSource,TEventArgs}"/> to configure the modules of the pipeline.</returns>
        public EventPipelineConfigurator<TSource, TEventArgs> IsForwardedToPipeline()
        {
            var pipeline = new Pipeline(m_EventsContext.Get<IServiceProvider>());

            m_SourceModelEventField.AddPipeline(pipeline);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                this
            );
        }
    }
}