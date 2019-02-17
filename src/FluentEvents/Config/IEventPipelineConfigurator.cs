using FluentEvents.Model;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public interface IEventPipelineConfigurator
    {
        SourceModel SourceModel { get; }
        SourceModelEventField SourceModelEventField { get; }
        EventsContext EventsContext { get; }
        Pipeline Pipeline { get; }
    }
}