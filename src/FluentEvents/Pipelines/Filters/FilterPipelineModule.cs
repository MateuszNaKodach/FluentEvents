using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Filters
{
    internal class FilterPipelineModule : IPipelineModule<FilterPipelineModuleConfig>
    {
        public Task InvokeAsync(
            FilterPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            return config.IsMatching(
                pipelineContext.PipelineEvent.OriginalSender,
                pipelineContext.PipelineEvent.OriginalEventArgs
            )
                ? invokeNextModule(pipelineContext)
                : Task.CompletedTask;
        }
    }
}
