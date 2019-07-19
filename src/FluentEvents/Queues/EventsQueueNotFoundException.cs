using System;

namespace FluentEvents.Queues
{
    /// <summary>
    ///     An exception that is thrown when the events queue with the specified name does not exists.
    /// </summary>
    [Serializable]
    public class EventsQueueNotFoundException : FluentEventsException
    {
        internal EventsQueueNotFoundException()
            : base("No queue was found with the specified name.")
        {
        }
    }
}