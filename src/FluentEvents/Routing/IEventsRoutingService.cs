using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Routing
{
    public interface IEventsRoutingService
    {
        Task RouteEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}