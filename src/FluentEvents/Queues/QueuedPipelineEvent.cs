using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public class QueuedPipelineEvent
    {
        public IPipeline Pipeline { get; set; }
        public PipelineEvent PipelineEvent { get; set; }
    }
}