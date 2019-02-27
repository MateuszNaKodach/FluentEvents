namespace FluentEvents.Routing
{
    public interface IAttachingInterceptor
    {
        void OnAttaching(object source, EventsScope eventsScope);
    }
}