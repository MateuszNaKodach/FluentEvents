using FluentEvents.Infrastructure;

namespace FluentEvents.Attachment
{
    /// <summary>
    ///     Allows interceptors to attach sources in the <see cref="IAttachingInterceptor.OnAttaching"/> method.
    /// </summary>
    /// <param name="source">The events source being attached.</param>
    /// <param name="eventsScope">The current <see cref="IEventsScope"/></param>
    public delegate void AttachDelegate(object source, IEventsScope eventsScope);
}
