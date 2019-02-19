using System;

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

        internal EventsScope EventsScope { get; }

        internal PipelineContext(PipelineEvent pipelineEvent, EventsScope eventsScope, IServiceProvider serviceProvider)
        {
            PipelineEvent = pipelineEvent ?? throw new ArgumentNullException(nameof(pipelineEvent));
            EventsScope = eventsScope ?? throw new ArgumentNullException(nameof(eventsScope));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
    }
}