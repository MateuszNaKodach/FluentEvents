using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event pipeline.
    /// </summary>
    public sealed class EventPipelineConfiguration<TEvent> : IInfrastructure<IServiceProvider>, IInfrastructure<IPipeline>
        where TEvent : class
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;
        IPipeline IInfrastructure<IPipeline>.Instance => _pipeline;

        private readonly IServiceProvider _serviceProvider;
        private readonly IPipeline _pipeline;

        /// <summary>
        ///     Creates an instance by taking dependencies from an <see cref="EventConfiguration{TEvent}"/>
        /// </summary>
        /// <param name="pipeline">The pipeline to configure.</param>
        /// <param name="eventConfiguration">The event configuration.</param>
        public EventPipelineConfiguration(
            IPipeline pipeline,
            EventConfiguration<TEvent> eventConfiguration
        )
        {
            if (eventConfiguration == null) throw new ArgumentNullException(nameof(eventConfiguration));

            _serviceProvider = eventConfiguration.Get<IServiceProvider>();
            _pipeline = pipeline;
        }

        /// <summary>
        ///     Allows to create an instance without passing an <see cref="EventConfiguration{TEvent}"/>
        /// </summary>
        /// <param name="serviceProvider">The events context.</param>
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