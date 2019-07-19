using System;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     An exception thrown when the configured pipeline module wasn't registered
    ///     in the internal <see cref="IServiceProvider"/>.
    /// </summary>
    [Serializable]
    public class PipelineModuleNotFoundException : FluentEventsException
    {
        internal PipelineModuleNotFoundException()
            : base($"Pipeline module not found. Please check if the required plugin is configured in the {nameof(EventsContextOptions)}")
        {
            
        }
    }
}