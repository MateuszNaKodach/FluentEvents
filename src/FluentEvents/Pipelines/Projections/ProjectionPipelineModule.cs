using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Projections
{
    public class ProjectionPipelineModule : IPipelineModule
    {
        public async Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule)
        {
            var config = (ProjectionPipelineModuleConfig)pipelineModuleContext.ModuleConfig;
            var proxySender = config.EventsSenderProjection.Convert(pipelineModuleContext.PipelineEvent.OriginalSender);
            var proxyEventArgs = config.EventArgsProjection.Convert(pipelineModuleContext.PipelineEvent.OriginalEventArgs);
            var projectedPipelineEvent = new PipelineEvent(
                pipelineModuleContext.PipelineEvent.OriginalSenderType,
                pipelineModuleContext.PipelineEvent.OriginalEventFieldName,
                proxySender,
                proxyEventArgs
            );
            
            pipelineModuleContext.PipelineEvent = projectedPipelineEvent;

            await invokeNextModule(pipelineModuleContext);
        }
    }
}