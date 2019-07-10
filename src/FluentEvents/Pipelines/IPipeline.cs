using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Represents an event pipeline.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        ///     Adds a module with the associated configuration to the pipeline.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <typeparam name="TConfig">The type of the module configuration.</typeparam>
        /// <param name="moduleConfig">An instance of the module configuration</param>
        void AddModule<TModule, TConfig>(TConfig moduleConfig) where TModule : IPipelineModule<TConfig>;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        Task ProcessEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}