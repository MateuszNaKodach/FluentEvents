using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public class QueuedPipelineEvent
    {
        public Pipeline Pipeline { get; set; }
        public PipelineEvent PipelineEvent { get; set; }
    }
}