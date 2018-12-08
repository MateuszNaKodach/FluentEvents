using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModule : IPipelineModule
    {
        private readonly IAzureSignalRClient m_AzureSignalRClient;

        public AzureSignalRPipelineModule(IAzureSignalRClient azureSignalRClient)
        {
            m_AzureSignalRClient = azureSignalRClient;
        }
        public async Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule)
        {
            var config = (AzureSignalRPipelineModuleConfig)pipelineModuleContext.ModuleConfig;

            await m_AzureSignalRClient.SendEventAsync(
                config.PublicationMethod,
                config.HubName,
                config.HubMethodName,
                config.SubjectIdsProvider(
                    pipelineModuleContext.PipelineEvent.OriginalSender,
                    pipelineModuleContext.PipelineEvent.OriginalEventArgs
                ),
                pipelineModuleContext.PipelineEvent.OriginalSender,
                pipelineModuleContext.PipelineEvent.OriginalEventArgs
            );

            await invokeNextModule(pipelineModuleContext);
        }
    }
}