using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Projections
{
    internal class ProjectionPipelineModule : IPipelineModule<ProjectionPipelineModuleConfig>
    {
        public Task InvokeAsync(
            ProjectionPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            var proxySender = config.EventsSenderProjection.Convert(pipelineContext.PipelineEvent.OriginalSender);
            var proxyEventArgs = config.EventArgsProjection.Convert(pipelineContext.PipelineEvent.OriginalEventArgs);
            var projectedPipelineEvent = new PipelineEvent(
                pipelineContext.PipelineEvent.OriginalSenderType,
                config.ProjectedEventFieldName,
                proxySender,
                proxyEventArgs
            );
            
            pipelineContext.PipelineEvent = projectedPipelineEvent;

            return invokeNextModule(pipelineContext);
        }
    }
}