using System;

namespace FluentEvents.Pipelines.Filters
{
    internal class FilterPipelineModuleConfig
    {
        internal Func<object, object, bool> IsMatching { get; }

        internal FilterPipelineModuleConfig(Func<object, object, bool> isMatching)
        {
            IsMatching = isMatching;
        }
    }
}
