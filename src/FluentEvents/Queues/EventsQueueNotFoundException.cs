namespace FluentEvents.Queues
{
    /// <summary>
    ///     An exception that is thrown when the events queue with the specified name does not exists.
    /// </summary>
    public class EventsQueueNotFoundException : FluentEventsException
    {
        internal EventsQueueNotFoundException()
            : base("No queue was found with the specified name.")
        {
        }
    }
}