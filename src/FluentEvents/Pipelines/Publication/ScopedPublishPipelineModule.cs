using System.Threading.Tasks;
using FluentEvents.Subscriptions;

namespace FluentEvents.Pipelines.Publication
{
    public class ScopedPublishPipelineModule : IPipelineModule<ScopedPublishPipelineModuleConfig>
    {
        private readonly IPublishingService m_PublishingService;

        public ScopedPublishPipelineModule(IPublishingService publishingService)
        {
            m_PublishingService = publishingService;
        }

        public async Task InvokeAsync(
            ScopedPublishPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            await m_PublishingService.PublishEventToScopedSubscriptionsAsync(
                pipelineContext.PipelineEvent, 
                pipelineContext.EventsScope
            );

            await invokeNextModule(pipelineContext);
        }
    }
}
