using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    public class GlobalPublishPipelineModule : IPipelineModule
    {
        private readonly IPublishingService m_PublishingService;
        private readonly Dictionary<Type, IEventSender> m_EventSenders;

        public GlobalPublishPipelineModule(IPublishingService publishingService, IEnumerable<IEventSender> eventSenders)
        {
            m_PublishingService = publishingService;
            m_EventSenders = eventSenders.ToDictionary(x => x.GetType(), x => x);
        }

        public async Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule)
        {
            var config = (GlobalPublishPipelineModuleConfig) pipelineModuleContext.ModuleConfig;

            if (config.SenderType != null)
                if (m_EventSenders.TryGetValue(config.SenderType, out var eventSender))
                    await eventSender.SendAsync(pipelineModuleContext.PipelineEvent);
                else
                    throw new EventSenderNotFoundException();
            else
                await m_PublishingService.PublishEventToGlobalSubscriptionsAsync(pipelineModuleContext.PipelineEvent);

            await invokeNextModule(pipelineModuleContext);
        }
    }
}
