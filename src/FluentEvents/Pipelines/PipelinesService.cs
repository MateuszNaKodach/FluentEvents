using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Utils;

namespace FluentEvents.Pipelines
{
    internal class PipelinesService : IPipelinesService
    {
        private readonly ConcurrentDictionary<Type, ConcurrentStack<IPipeline>> _pipelines;

        public PipelinesService()
        {
            _pipelines = new ConcurrentDictionary<Type, ConcurrentStack<IPipeline>>();
        }

        public void AddPipeline(Type eventType, IPipeline pipeline)
        {
            _pipelines.AddOrUpdate(eventType, type => new ConcurrentStack<IPipeline>(new[] {pipeline}), (type, stack) =>
            {
                stack.Push(pipeline);
                return stack;
            });
        }

        public IEnumerable<IPipeline> GetPipelines(Type eventType)
        {
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));

            foreach (var type in eventType.GetBaseTypesAndInterfacesInclusive())
                if (_pipelines.TryGetValue(type, out var pipelines))
                    foreach (var pipeline in pipelines)
                        yield return pipeline;
        }
    }
}
