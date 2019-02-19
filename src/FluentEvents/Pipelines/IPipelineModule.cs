using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Represents a module for event pipelines.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration.</typeparam>
    public interface IPipelineModule<in TConfig>
    {
        /// <summary>
        ///     This method should work with the event and invoke the <see cref="NextModuleDelegate"/>
        ///     passed in the parameters if and when the event processing should continue. 
        /// </summary>
        /// <param name="config">The configuration for the particular event being processed.</param>
        /// <param name="pipelineContext">The current pipeline processing context.</param>
        /// <param name="invokeNextModule">The delegate to invoke the next module in the pipeline.</param>
        /// <returns>A task to await.</returns>
        Task InvokeAsync(TConfig config, PipelineContext pipelineContext, NextModuleDelegate invokeNextModule);
    }
}