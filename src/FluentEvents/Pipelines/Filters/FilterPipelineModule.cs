using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Filters
{
    public class FilterPipelineModule : IPipelineModule<FilterPipelineModuleConfig>
    {
        public async Task InvokeAsync(
            FilterPipelineModuleConfig config,
            PipelineContext pipelineContext, 
            NextModuleDelegate invokeNextModule
        )
        {
            if (config.IsMatching(pipelineContext.PipelineEvent.OriginalSender, pipelineContext.PipelineEvent.OriginalEventArgs))
                await invokeNextModule(pipelineContext);
        }
    }
}
