using System;

namespace FluentEvents.Pipelines
{
    public class PipelineModuleContext : PipelineContext
    {
        public object ModuleConfig { get; }

        public PipelineModuleContext(
            object moduleConfig,
            PipelineContext pipelineContext
        ) 
            : base(pipelineContext.PipelineEvent, pipelineContext.EventsScope, pipelineContext.ServiceProvider)
        {
            ModuleConfig = moduleConfig ?? throw new ArgumentNullException(nameof(moduleConfig));
        }
    }
}
