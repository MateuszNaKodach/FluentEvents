using System.Threading.Tasks;
using FluentEvents.Queues;

namespace FluentEvents.Pipelines.Queues
{
    internal class EnqueuePipelineModule : IPipelineModule<EnqueuePipelineModuleConfig>
    {
        private readonly IEventsQueuesService _eventsQueuesService;

        public EnqueuePipelineModule(IEventsQueuesService eventsQueuesService)
        {
            _eventsQueuesService = eventsQueuesService;
        }
        
        public Task InvokeAsync(
            EnqueuePipelineModuleConfig config, 
            PipelineContext pipelineContext,
            NextModuleDelegate invokeNextModule
        )
        {
            _eventsQueuesService.EnqueueEvent(
                pipelineContext.EventsScope,
                pipelineContext.PipelineEvent,
                config.QueueName,
                () => invokeNextModule(pipelineContext)
            );

            return Task.CompletedTask;
        }
    }
}
