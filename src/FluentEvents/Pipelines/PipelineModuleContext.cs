using System;

namespace FluentEvents.Pipelines
{
    public class PipelineModuleContext : PipelineContext
    {
        public IPipelineModuleConfig ModuleConfig { get; }

        public PipelineModuleContext(
            IPipelineModuleConfig moduleConfig,
            PipelineContext pipelineContext
        ) 
            : base(pipelineContext.PipelineEvent, pipelineContext.EventsScope, pipelineContext.ServiceProvider)
        {
            ModuleConfig = moduleConfig ?? throw new ArgumentNullException(nameof(moduleConfig));
        }
    }
}
