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
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;
        SourceModel IInfrastructure<SourceModel>.Instance => _sourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => _sourceModelEventField;
        IPipeline IInfrastructure<IPipeline>.Instance => _pipeline;

        private readonly IServiceProvider _serviceProvider;
        private readonly SourceModel _sourceModel;
        private readonly SourceModelEventField _sourceModelEventField;
        private readonly IPipeline _pipeline;

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
            _sourceModel = eventConfigurator.Get<SourceModel>();
            _sourceModelEventField = eventConfigurator.Get<SourceModelEventField>();
            _serviceProvider = eventConfigurator.Get<IServiceProvider>();
            _pipeline = pipeline;
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
            _sourceModel = sourceModel;
            _sourceModelEventField = sourceModelEventField;
            _serviceProvider = serviceProvider;
            _pipeline = pipeline;
        }
    }
}