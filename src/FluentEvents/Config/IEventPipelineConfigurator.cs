using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public interface IEventPipelineConfigurator : IEventConfigurator
    {
        Pipeline Pipeline { get; }
    }
}