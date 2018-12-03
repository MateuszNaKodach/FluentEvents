using FluentEvents.Model;

namespace FluentEvents.Routing
{
    public interface IForwardingService
    {
        void ForwardEventsToRouting(SourceModel sourceModel, object source, EventsScope eventsScope);
    }
}