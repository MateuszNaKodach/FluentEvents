using System.Threading.Tasks;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModule : IPipelineModule<AzureSignalRPipelineModuleConfig>
    {
        private readonly IAzureSignalRClient _azureSignalRClient;

        public AzureSignalRPipelineModule(IAzureSignalRClient azureSignalRClient)
        {
            _azureSignalRClient = azureSignalRClient;
        }

        public async Task InvokeAsync(
            AzureSignalRPipelineModuleConfig config, 
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            await _azureSignalRClient.SendEventAsync(
                config.PublicationMethod,
                config.HubName,
                config.HubMethodName,
                config.ReceiverIdsProviderAction?.Invoke(
                    pipelineContext.PipelineEvent.OriginalSender,
                    pipelineContext.PipelineEvent.OriginalEventArgs
                ),
                pipelineContext.PipelineEvent.OriginalSender,
                pipelineContext.PipelineEvent.OriginalEventArgs
            );

            await invokeNextModule(pipelineContext);
        }
    }
}