using System;

namespace FluentEvents.Pipelines.Filters
{
    internal class FilterPipelineModuleConfig
    {
        internal Func<object, bool> IsMatching { get; }

        internal FilterPipelineModuleConfig(Func<object, bool> isMatching)
        {
            IsMatching = isMatching;
        }
    }
}
