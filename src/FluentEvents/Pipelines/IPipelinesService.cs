using System;
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
        /// <exception cref="ArgumentNullException">
        ///      <paramref name="eventType"/> is <see langword="null"/>.
        /// </exception>
        IEnumerable<IPipeline> GetPipelines(Type eventType);
    }
}