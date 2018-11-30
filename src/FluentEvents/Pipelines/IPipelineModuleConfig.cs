using System;

namespace FluentEvents.Pipelines
{
    public interface IPipelineModuleConfig
    {
        Type ModuleType { get; }
    }
}