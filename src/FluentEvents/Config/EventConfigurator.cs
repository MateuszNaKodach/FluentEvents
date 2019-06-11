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
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;
        SourceModel IInfrastructure<SourceModel>.Instance => _sourceModel;
        SourceModelEventField IInfrastructure<SourceModelEventField>.Instance => _sourceModelEventField;

        private readonly IServiceProvider _serviceProvider;
        private readonly SourceModel _sourceModel;
        private readonly SourceModelEventField _sourceModelEventField;

        internal EventConfigurator(
            IServiceProvider serviceProvider,
            SourceModel sourceModel,
            SourceModelEventField sourceModelEventField
        )
        {
            _sourceModel = sourceModel;
            _sourceModelEventField = sourceModelEventField;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This method creates a pipeline for the current event.
        /// </summary>
        /// <returns>
        ///     An <see cref="EventPipelineConfigurator{TSource,TEventArgs}"/> to configure the modules of the pipeline.
        /// </returns>
        public EventPipelineConfigurator<TSource, TEventArgs> IsWatched()
        {
            var pipeline = new Pipeline(_serviceProvider);

            _sourceModelEventField.AddPipeline(pipeline);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                this
            );
        }
    }
}