using System;
using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     An exception thrown when the configured <see cref="IEventSender"/>
    ///     wasn't registered in the internal <see cref="IServiceProvider"/>.
    /// </summary>
    [Serializable]
    public class EventSenderNotFoundException : FluentEventsException
    {
        internal EventSenderNotFoundException()
            : base($"The configured event sender wasn't registered in the internal {nameof(IServiceProvider)}")
        {
        }
    }
}