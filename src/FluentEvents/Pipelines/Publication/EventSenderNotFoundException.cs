using System;
using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     An exception thrown when the configured <see cref="IEventSender"/>
    ///     wasn't registered in the internal <see cref="IServiceProvider"/>.
    /// </summary>
    public class EventSenderNotFoundException : FluentEventsException
    {
    }
}