using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal class QueuedPipelineEvent
    {
        public Func<Task> InvokeNextModule { get; }
        public PipelineEvent PipelineEvent { get; }

        public QueuedPipelineEvent(Func<Task> invokeNextModule, PipelineEvent pipelineEvent)
        {
            InvokeNextModule = invokeNextModule;
            PipelineEvent = pipelineEvent;
        }
    }
}