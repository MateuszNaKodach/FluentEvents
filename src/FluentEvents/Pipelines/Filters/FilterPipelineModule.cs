using System.Threading.Tasks;

namespace FluentEvents.Pipelines.Filters
{
    public class FilterPipelineModule : IPipelineModule
    {
        public async Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule)
        {
            var config = (FilterPipelineModuleConfig)pipelineModuleContext.ModuleConfig;

            if (config.IsMatching(pipelineModuleContext.PipelineEvent.OriginalSender, pipelineModuleContext.PipelineEvent.OriginalEventArgs))
                await invokeNextModule(pipelineModuleContext);
        }
    }
}
