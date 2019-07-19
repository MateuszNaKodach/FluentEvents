using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event.
    /// </summary>
    public sealed class EventConfigurator<TEvent> : IInfrastructure<IServiceProvider>
        where TEvent : class 
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;

        private readonly IServiceProvider _serviceProvider;

        internal EventConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This method creates a pipeline for the current event.
        /// </summary>
        /// <returns>
        ///     An <see cref="EventPipelineConfigurator{TEventArgs}"/> to configure the modules of the pipeline.
        /// </returns>
        public EventPipelineConfigurator<TEvent> IsPiped()
        {
            var pipeline = new Pipeline(_serviceProvider);

            var pipelinesService = _serviceProvider.GetRequiredService<IPipelinesService>();

            pipelinesService.AddPipeline(typeof(TEvent), pipeline);

            return new EventPipelineConfigurator<TEvent>(pipeline, this);
        }
    }
}