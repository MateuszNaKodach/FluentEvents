using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    public delegate Task NextModuleDelegate(PipelineContext pipelineContext);
}
