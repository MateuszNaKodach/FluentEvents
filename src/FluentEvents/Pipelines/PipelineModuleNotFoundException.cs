using System;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     An exception thrown when the configured pipeline module wasn't registered
    ///     in the internal <see cref="IServiceProvider"/>.
    /// </summary>
    public class PipelineModuleNotFoundException : FluentEventsException
    {
    }
}