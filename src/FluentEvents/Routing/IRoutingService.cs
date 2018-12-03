using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    public interface IRoutingService
    {
        Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}