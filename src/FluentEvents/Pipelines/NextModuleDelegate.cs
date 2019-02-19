using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Invokes the next module in the pipeline if exists, otherwise it does nothing.
    /// </summary>
    /// <param name="pipelineContext">The current pipeline processing context.</param>
    /// <returns>An awaitable task.</returns>
    public delegate Task NextModuleDelegate(PipelineContext pipelineContext);
}
