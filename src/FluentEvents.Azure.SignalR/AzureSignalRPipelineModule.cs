using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModule : IPipelineModule<AzureSignalRPipelineModuleConfig>
    {
        private readonly IAzureSignalRClient m_AzureSignalRClient;

        public AzureSignalRPipelineModule(IAzureSignalRClient azureSignalRClient)
        {
            m_AzureSignalRClient = azureSignalRClient;
        }

        public async Task InvokeAsync(
            AzureSignalRPipelineModuleConfig config, 
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            await m_AzureSignalRClient.SendEventAsync(
                config.PublicationMethod,
                config.HubName,
                config.HubMethodName,
                config.SubjectIdsProvider(
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