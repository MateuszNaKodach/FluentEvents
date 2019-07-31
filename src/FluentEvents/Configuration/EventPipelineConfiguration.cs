using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Allows configuration to be performed for an event pipeline.
    /// </summary>
    public sealed class EventPipelineConfiguration<TEvent> : IInfrastructure<IServiceProvider>, IInfrastructure<IPipeline>
        where TEvent : class
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;
        IPipeline IInfrastructure<IPipeline>.Instance => _pipeline;

        private readonly IServiceProvider _serviceProvider;
        private readonly IPipeline _pipeline;

        internal EventPipelineConfiguration(
            IPipeline pipeline,
            EventConfiguration<TEvent> eventConfiguration
        )
        {
            _serviceProvider = eventConfiguration.Get<IServiceProvider>();
            _pipeline = pipeline;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="EventPipelineConfiguration{TEvent}"/>
        /// </summary>
        /// <param name="serviceProvider">The internal service provider.</param>
        /// <param name="pipeline">The pipeline to configure.</param>
        public EventPipelineConfiguration(
            IServiceProvider serviceProvider,
            IPipeline pipeline
        )
        {
            _serviceProvider = serviceProvider;
            _pipeline = pipeline;
        }
    }
}