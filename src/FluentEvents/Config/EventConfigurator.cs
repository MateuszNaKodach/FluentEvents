using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event.
    /// </summary>
    public class EventConfigurator<TSource, TEventArgs> 
        : IInfrastructure<IServiceProvider>,
            IInfrastructure<SourceModel>,
            IInfrastructure<SourceModelEventField>
        where TSource : class 
        where TEventArgs : class 
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => m_ServiceProvider;
        SourceModel IInfrastructure<SourceModel>.Instance => m_SourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => m_SourceModelEventField;

        private readonly IServiceProvider m_ServiceProvider;
        private readonly SourceModel m_SourceModel;
        private readonly SourceModelEventField m_SourceModelEventField;

        internal EventConfigurator(
            IServiceProvider serviceProvider,
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField
        )
        {
            m_SourceModel = sourceModel;
            m_SourceModelEventField = sourceModelEventField;
            m_ServiceProvider = serviceProvider;
        }

        /// <summary>
        ///     This method creates a pipeline for the current event.
        /// </summary>
        /// <returns>
        ///     An <see cref="EventPipelineConfigurator{TSource,TEventArgs}"/> to configure the modules of the pipeline.
        /// </returns>
        public EventPipelineConfigurator<TSource, TEventArgs> IsForwardedToPipeline()
        {
            var pipeline = new Pipeline(m_ServiceProvider);

            m_SourceModelEventField.AddPipeline(pipeline);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                this
            );
        }
    }
}