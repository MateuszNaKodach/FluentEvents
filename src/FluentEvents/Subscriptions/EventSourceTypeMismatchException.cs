namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     An exception that is thrown when the source of the event to publish
    ///     is not an instance of <see cref="Subscription.SourceType"/>.
    /// </summary>
    public class EventSourceTypeMismatchException : FluentEventsException
    {
    }
}