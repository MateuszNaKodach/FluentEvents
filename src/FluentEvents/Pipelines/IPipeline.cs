using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    public interface IPipeline
    {
        Task ProcessEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}