using FluentEvents.Infrastructure;

namespace FluentEvents.Attachment
{
    /// <summary>
    ///     This interface can be implemented to intercept every attaching made
    ///     to the <see cref="EventsContext"/>.
    /// </summary>
    public interface IAttachingInterceptor
    {
        /// <summary>
        ///     This method can be used to handle complex attaching situations
        ///     (For example attaching to the entities of an ORM).
        /// </summary>
        /// <param name="attach">A delegate that allows the interceptor to attach sources.</param>
        /// <param name="source">The events source being attached.</param>
        /// <param name="eventsScope">The current <see cref="IEventsScope"/></param>
        void OnAttaching(AttachDelegate attach, object source, IEventsScope eventsScope);
    }
}