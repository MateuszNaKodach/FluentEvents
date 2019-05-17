using System.Threading.Tasks;
using FluentEvents.Subscriptions;

namespace FluentEvents.Pipelines.Publication
{
    internal class ScopedPublishPipelineModule : IPipelineModule<ScopedPublishPipelineModuleConfig>
    {
        private readonly IPublishingService _publishingService;

        public ScopedPublishPipelineModule(IPublishingService publishingService)
        {
            _publishingService = publishingService;
        }

        public async Task InvokeAsync(
            ScopedPublishPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            await _publishingService.PublishEventToScopedSubscriptionsAsync(
                pipelineContext.PipelineEvent, 
                pipelineContext.EventsScope
            );

            await invokeNextModule(pipelineContext);
        }
    }
}
