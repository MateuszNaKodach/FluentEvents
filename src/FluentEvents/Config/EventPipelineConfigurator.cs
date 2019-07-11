using System;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface for configuring an event pipeline.
    /// </summary>
    public class EventPipelineConfigurator<TEvent> : IInfrastructure<IServiceProvider>, IInfrastructure<IPipeline>
        where TEvent : class
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;
        IPipeline IInfrastructure<IPipeline>.Instance => _pipeline;

        private readonly IServiceProvider _serviceProvider;
        private readonly IPipeline _pipeline;

        /// <summary>
        ///     Creates an instance by taking dependencies from an <see cref="EventConfigurator{TEvent}"/>
        /// </summary>
        /// <param name="pipeline">The pipeline to configure.</param>
        /// <param name="eventConfigurator">The event configurator.</param>
        public EventPipelineConfigurator(
            IPipeline pipeline,
            EventConfigurator<TEvent> eventConfigurator
        )
        {
            _serviceProvider = eventConfigurator.Get<IServiceProvider>();
            _pipeline = pipeline;
        }

        /// <summary>
        ///     Allows to create an instance without passing an <see cref="EventConfigurator{TEvent}"/>
        /// </summary>
        /// <param name="serviceProvider">The events context.</param>
        /// <param name="pipeline">The pipeline to configure.</param>
        public EventPipelineConfigurator(
            IServiceProvider serviceProvider,
            IPipeline pipeline
        )
        {
            _serviceProvider = serviceProvider;
            _pipeline = pipeline;
        }
    }
}