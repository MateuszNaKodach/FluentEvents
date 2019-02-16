using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public class QueuedPipelineEvent
    {
        public Func<Task> InvokeNextModule { get; set; }
        public PipelineEvent PipelineEvent { get; set; }
    }
}