using System.Threading.Tasks;
using FluentEvents.Queues;

namespace FluentEvents.Pipelines.Queues
{
    internal class EnqueuePipelineModule : IPipelineModule<EnqueuePipelineModuleConfig>
    {
        private readonly IEventsQueuesService m_EventsQueuesService;

        public EnqueuePipelineModule(IEventsQueuesService eventsQueuesService)
        {
            m_EventsQueuesService = eventsQueuesService;
        }
        
        public Task InvokeAsync(
            EnqueuePipelineModuleConfig config, 
            PipelineContext pipelineContext,
            NextModuleDelegate invokeNextModule
        )
        {
            m_EventsQueuesService.EnqueueEvent(
                pipelineContext.EventsScope,
                pipelineContext.PipelineEvent,
                config.QueueName,
                () => invokeNextModule(pipelineContext)
            );

            return Task.CompletedTask;
        }
    }
}
