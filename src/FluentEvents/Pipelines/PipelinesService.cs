using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IPipelinesService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void AddPipeline(Type eventType, IPipeline pipeline);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<IPipeline> GetPipelines(Type eventType);
    }

    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class PipelinesService : IPipelinesService
    {
        private readonly ConcurrentDictionary<Type, ConcurrentStack<IPipeline>> _pipelines;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PipelinesService()
        {
            _pipelines = new ConcurrentDictionary<Type, ConcurrentStack<IPipeline>>();
        }

        /// <inheritdoc />
        public void AddPipeline(Type eventType, IPipeline pipeline)
        {
            _pipelines.AddOrUpdate(eventType, type => new ConcurrentStack<IPipeline>(new[] {pipeline}), (type, stack) =>
            {
                stack.Push(pipeline);
                return stack;
            });
        }

        /// <inheritdoc />
        public IEnumerable<IPipeline> GetPipelines(Type eventType)
        {
            if (_pipelines.TryGetValue(eventType, out var pipelines))
                foreach (var pipeline in pipelines)
                    yield return pipeline;
        }
    }
}
