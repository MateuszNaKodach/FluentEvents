using System;

namespace FluentEvents.Pipelines.Filters
{
    public class FilterPipelineModuleConfig : PipelineModuleConfig<FilterPipelineModule>
    {
        public Func<object, object, bool> IsMatching { get; }

        public FilterPipelineModuleConfig(Func<object, object, bool> isMatching)
        {
            IsMatching = isMatching;
        }
    }
}
