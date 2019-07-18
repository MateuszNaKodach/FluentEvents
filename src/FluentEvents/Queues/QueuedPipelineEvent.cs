using System;
using System.Threading.Tasks;

namespace FluentEvents.Queues
{
    internal class QueuedPipelineEvent
    {
        internal Func<Task> InvokeNextModule { get; }

        internal QueuedPipelineEvent(Func<Task> invokeNextModule)
        {
            InvokeNextModule = invokeNextModule;
        }
    }
}