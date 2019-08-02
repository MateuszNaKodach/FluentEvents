using System;

namespace FluentEvents.Subscriptions
{
    [Serializable]
    internal class EventTypeMismatchException : FluentEventsException
    {
        internal EventTypeMismatchException() 
            : base($"The event source type doesn't match the {nameof(Subscription)}.{nameof(Subscription.EventType)}")
        {
            
        }
    }
}