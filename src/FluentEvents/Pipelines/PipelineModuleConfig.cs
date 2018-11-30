using System;

namespace FluentEvents.Pipelines
{
    public abstract class PipelineModuleConfig<T> : IPipelineModuleConfig where T : IPipelineModule
    {
        public Type ModuleType => typeof(T);
    }
}