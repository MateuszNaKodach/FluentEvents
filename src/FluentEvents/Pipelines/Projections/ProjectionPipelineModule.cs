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
            var projectedEvent = config.EventProjection.Convert(pipelineContext.PipelineEvent.Event);

            var projectedPipelineEvent = new PipelineEvent(projectedEvent);
            
            pipelineContext.PipelineEvent = projectedPipelineEvent;

            return invokeNextModule(pipelineContext);
        }
    }
}