using System;
using FluentEvents.Infrastructure;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Represents a pipeline processing context. 
    /// </summary>
    public class PipelineContext
    {
        /// <summary>
        ///     The event being processed.
        /// </summary>
        public PipelineEvent PipelineEvent { get; set; }

        /// <summary>
        ///     The internal service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        ///     The current events scope.
        /// </summary>
        public IEventsScope EventsScope { get; }

        /// <summary>
        ///     Creates a new instance of a <see cref="PipelineContext"/>.
        /// </summary>
        /// <param name="pipelineEvent">The event being processed.</param>
        /// <param name="eventsScope">The current events scope.</param>
        /// <param name="serviceProvider">The internal service provider.</param>
        public PipelineContext(PipelineEvent pipelineEvent, IEventsScope eventsScope, IServiceProvider serviceProvider)
        {
            PipelineEvent = pipelineEvent ?? throw new ArgumentNullException(nameof(pipelineEvent));
            EventsScope = eventsScope ?? throw new ArgumentNullException(nameof(eventsScope));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
    }
}