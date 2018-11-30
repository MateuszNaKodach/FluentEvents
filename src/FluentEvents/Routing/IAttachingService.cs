namespace FluentEvents.Routing
{
    public interface IAttachingService
    {
        void Attach(object source, EventsScope eventsScope);
    }
}