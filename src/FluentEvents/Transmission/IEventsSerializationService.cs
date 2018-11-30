using FluentEvents.Pipelines;

namespace FluentEvents.Transmission
{
    public interface IEventsSerializationService
    {
        string SerializeEvent(PipelineEvent pipelineEvent);
        PipelineEvent Deserialize(string data);
    }
}