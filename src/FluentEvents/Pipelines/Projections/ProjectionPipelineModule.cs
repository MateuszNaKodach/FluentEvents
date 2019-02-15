using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Projections
{
    public class ProjectionPipelineModule : IPipelineModule<ProjectionPipelineModuleConfig>
    {
        public async Task InvokeAsync(
            ProjectionPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            var proxySender = config.EventsSenderProjection.Convert(pipelineContext.PipelineEvent.OriginalSender);
            var proxyEventArgs = config.EventArgsProjection.Convert(pipelineContext.PipelineEvent.OriginalEventArgs);
            var projectedPipelineEvent = new PipelineEvent(
                pipelineContext.PipelineEvent.OriginalSenderType,
                pipelineContext.PipelineEvent.OriginalEventFieldName,
                proxySender,
                proxyEventArgs
            );
            
            pipelineContext.PipelineEvent = projectedPipelineEvent;

            await invokeNextModule(pipelineContext);
        }
    }
}