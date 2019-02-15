using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    public interface IPipelineModule<in TConfig>
    {
        Task InvokeAsync(TConfig config, PipelineContext pipelineContext, NextModuleDelegate invokeNextModule);
    }
}