using System;

namespace FluentEvents.Pipelines.Publication
{
    public class GlobalPublishPipelineModuleConfig : PipelineModuleConfig<GlobalPublishPipelineModule>
    {
        public Type SenderType { get; set; }
    }
}
