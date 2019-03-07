using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event pipeline.
    /// </summary>
    public class EventPipelineConfigurator<TSource, TEventArgs>
        : IInfrastructure<IServiceProvider>,
            IInfrastructure<SourceModel>,
            IInfrastructure<SourceModelEventField>,
            IInfrastructure<IPipeline>
        where TSource : class
        where TEventArgs : class
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => m_ServiceProvider;
        SourceModel IInfrastructure<SourceModel>.Instance => m_SourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => m_SourceModelEventField;
        IPipeline IInfrastructure<IPipeline>.Instance => m_Pipeline;

        private readonly IServiceProvider m_ServiceProvider;
        private readonly SourceModel m_SourceModel;
        private readonly SourceModelEventField m_SourceModelEventField;
        private readonly IPipeline m_Pipeline;

        /// <summary>
        ///     Creates an instance by taking dependencies from an <see cref="EventConfigurator{TSource,TEventArgs}"/>
        /// </summary>
        /// <param name="pipeline">The pipeline to configure.</param>
        /// <param name="eventConfigurator">The event configurator.</param>
        public EventPipelineConfigurator(
            IPipeline pipeline,
            EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
        {
            m_SourceModel = eventConfigurator.Get<SourceModel>();
            m_SourceModelEventField = eventConfigurator.Get<SourceModelEventField>();
            m_ServiceProvider = eventConfigurator.Get<IServiceProvider>();
            m_Pipeline = pipeline;
        }

        /// <summary>
        ///     Allows to create an instance without passing an <see cref="EventConfigurator{TSource,TEventArgs}"/>
        /// </summary>
        /// <param name="sourceModel">The source model.</param>
        /// <param name="sourceModelEventField">The source model event field.</param>
        /// <param name="serviceProvider">The events context.</param>
        /// <param name="pipeline">The pipeline to configure.</param>
        public EventPipelineConfigurator(
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField,
            IServiceProvider serviceProvider,
            IPipeline pipeline
        )
        {
            m_SourceModel = sourceModel;
            m_SourceModelEventField = sourceModelEventField;
            m_ServiceProvider = serviceProvider;
            m_Pipeline = pipeline;
        }
    }
}