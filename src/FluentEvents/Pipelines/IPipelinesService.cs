using System;
using System.Collections.Generic;

namespace FluentEvents.Pipelines
{
    internal interface IPipelinesService
    {
        void AddPipeline(Type eventType, IPipeline pipeline);

        IEnumerable<IPipeline> GetPipelines(Type eventType);
    }
}