using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Attachment
{
    internal interface IRoutingService
    {
        Task RouteEventAsync(PipelineEvent pipelineEvent, IEventsScope eventsScope);
    }
}