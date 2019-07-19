using System;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     An exception that is thrown when the source of the event to publish
    ///     is not an instance of <see cref="Subscription.EventType"/>.
    /// </summary>
    [Serializable]
    public class EventTypeMismatchException : FluentEventsException
    {
        internal EventTypeMismatchException() 
            : base($"The event source type doesn't match the {nameof(Subscription)}.{nameof(Subscription.EventType)}")
        {
            
        }
    }
}