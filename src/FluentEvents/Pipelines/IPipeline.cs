using System.Threading.Tasks;

namespace FluentEvents.Pipelines
{
    public interface IPipeline
    {
        string QueueName { get; }
        IEventsContext EventsContext { get; }

        Task ProcessEventAsync(PipelineEvent pipelineEvent, EventsScope eventsScope);
    }
}