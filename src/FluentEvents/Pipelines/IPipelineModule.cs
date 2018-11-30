using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    public interface IPipelineModule
    {
        Task InvokeAsync(PipelineModuleContext pipelineModuleContext, NextModuleDelegate invokeNextModule);
    }
}