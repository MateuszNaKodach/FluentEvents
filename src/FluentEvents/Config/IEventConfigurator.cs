using FluentEvents.Model;

namespace FluentEvents.Config
{
    public interface IEventConfigurator
    {
        SourceModel SourceModel { get; }
        SourceModelEventField SourceModelEventField { get; }
        EventsContext EventsContext { get; }
    }
}