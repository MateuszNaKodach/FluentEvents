using System;

namespace FluentEvents.Pipelines
{
    public class PipelineContext
    {
        public PipelineEvent PipelineEvent { get; set; }
        public IServiceProvider ServiceProvider { get; }
        internal EventsScope EventsScope { get; }

        public PipelineContext(PipelineEvent pipelineEvent, EventsScope eventsScope, IServiceProvider serviceProvider)
        {
            PipelineEvent = pipelineEvent ?? throw new ArgumentNullException(nameof(pipelineEvent));
            EventsScope = eventsScope ?? throw new ArgumentNullException(nameof(eventsScope));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
    }
}