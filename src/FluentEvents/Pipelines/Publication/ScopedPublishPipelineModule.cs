using System.Threading.Tasks;
using FluentEvents.Publication;
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
            ).ConfigureAwait(false);

            await invokeNextModule(pipelineContext).ConfigureAwait(false);
        }
    }
}
