using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModule : IPipelineModule<AzureSignalRPipelineModuleConfig>
    {
        private readonly IEventSendingService _eventSendingService;

        public AzureSignalRPipelineModule(IEventSendingService eventSendingService)
        {
            _eventSendingService = eventSendingService;
        }

        public async Task InvokeAsync(
            AzureSignalRPipelineModuleConfig config, 
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            await _eventSendingService.SendEventAsync(
                config.PublicationMethod,
                config.HubName,
                config.HubMethodName,
                config.ReceiverIdsProviderAction?.Invoke(
                    pipelineContext.PipelineEvent.OriginalSender,
                    pipelineContext.PipelineEvent.Event
                ),
                pipelineContext.PipelineEvent.OriginalSender,
                pipelineContext.PipelineEvent.Event
            ).ConfigureAwait(false);

            await invokeNextModule(pipelineContext).ConfigureAwait(false);
        }
    }
}