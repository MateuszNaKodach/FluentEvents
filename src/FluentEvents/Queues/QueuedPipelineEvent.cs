using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal class QueuedPipelineEvent
    {
        public Func<Task> InvokeNextModule { get; set; }
        public PipelineEvent PipelineEvent { get; set; }
    }
}