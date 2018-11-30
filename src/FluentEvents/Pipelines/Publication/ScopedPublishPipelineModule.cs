using System.Threading.Tasks;
using FluentEvents.Subscriptions;

namespace FluentEvents.Pipelines.Publication
{
    public class ScopedPublishPipelineModule : IPipelineModule
    {
        private readonly IPublishingService m_PublishingService;

        public ScopedPublishPipelineModule(IPublishingService publishingService)
        {
            m_PublishingService = publishingService;
        }

        public async Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule)
        {
            await m_PublishingService.PublishEventToScopedSubscriptionsAsync(
                pipelineModuleContext.PipelineEvent, 
                pipelineModuleContext.EventsScope
            );

            await invokeNextModule(pipelineModuleContext);
        }
    }
}
