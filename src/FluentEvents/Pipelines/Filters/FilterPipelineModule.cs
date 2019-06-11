using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Filters
{
    internal class FilterPipelineModule : IPipelineModule<FilterPipelineModuleConfig>
    {
        public async Task InvokeAsync(
            FilterPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            if (config.IsMatching(pipelineContext.PipelineEvent.OriginalSender, pipelineContext.PipelineEvent.OriginalEventArgs))
                await invokeNextModule(pipelineContext).ConfigureAwait(false);
        }
    }
}
