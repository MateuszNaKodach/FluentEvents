using FluentEvents.Infrastructure;

namespace FluentEvents.Attachment
{
    internal interface IAttachingService
    {
        void Attach(object source, IEventsScope eventsScope);
    }
}