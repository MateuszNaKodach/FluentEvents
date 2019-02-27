namespace FluentEvents.Routing
{
    public interface IAttachingInterceptor
    {
        void OnAttaching(IAttachingService attachingService, object source, EventsScope eventsScope);
    }
}