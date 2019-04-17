namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception thrown when the event selection action doesn't subscribes correctly the dynamic object.
    /// </summary>
    public class NoEventsSelectedException : FluentEventsException
    {
        internal NoEventsSelectedException()
            : base("The event selection action doesn't subscribe the provided dynamic object to any event.")
        {
            
        }
    }
}