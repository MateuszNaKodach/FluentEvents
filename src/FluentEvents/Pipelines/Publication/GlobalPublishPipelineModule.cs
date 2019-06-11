using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    internal class GlobalPublishPipelineModule : IPipelineModule<GlobalPublishPipelineModuleConfig>
    {
        private readonly IPublishingService _publishingService;
        private readonly Dictionary<Type, IEventSender> _eventSenders;

        public GlobalPublishPipelineModule(IPublishingService publishingService, IEnumerable<IEventSender> eventSenders)
        {
            _publishingService = publishingService;
            _eventSenders = eventSenders.ToDictionary(x => x.GetType(), x => x);
        }

        public async Task InvokeAsync(
            GlobalPublishPipelineModuleConfig config,
            PipelineContext pipelineContext,
            NextModuleDelegate invokeNextModule
        )
        {
            if (config.SenderType != null)
                if (_eventSenders.TryGetValue(config.SenderType, out var eventSender))
                    await eventSender.SendAsync(pipelineContext.PipelineEvent).ConfigureAwait(false);
                else
                    throw new EventSenderNotFoundException();
            else
                await _publishingService.PublishEventToGlobalSubscriptionsAsync(pipelineContext.PipelineEvent).ConfigureAwait(false);

            await invokeNextModule(pipelineContext).ConfigureAwait(false);
        }
    }
}
