using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IRoutingService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}