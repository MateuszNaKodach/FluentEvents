using FluentEvents.Infrastructure;
using FluentEvents.Model;

namespace FluentEvents.Routing
{
    internal interface IForwardingService
    {
        void ForwardEventsToRouting(SourceModel sourceModel, object source, IEventsScope eventsScope);
    }
}