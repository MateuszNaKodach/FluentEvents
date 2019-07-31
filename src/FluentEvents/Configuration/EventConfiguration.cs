﻿using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Allows configuration to be performed for an event type.
    /// </summary>
    public sealed class EventConfiguration<TEvent> : IInfrastructure<IServiceProvider>
        where TEvent : class 
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => _serviceProvider;

        private readonly IServiceProvider _serviceProvider;

        internal EventConfiguration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This method creates a pipeline for the current event.
        /// </summary>
        /// <returns>
        ///     An <see cref="EventPipelineConfiguration{TEvent}"/> to configure the modules of the pipeline.
        /// </returns>
        public EventPipelineConfiguration<TEvent> IsPiped()
        {
            var pipeline = new Pipeline(_serviceProvider);

            var pipelinesService = _serviceProvider.GetRequiredService<IPipelinesService>();

            pipelinesService.AddPipeline(typeof(TEvent), pipeline);

            return new EventPipelineConfiguration<TEvent>(pipeline, this);
        }
    }
}