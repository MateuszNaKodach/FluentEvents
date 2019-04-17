namespace FluentEvents.Config
{
    /// <summary>
    ///     An exception throw when configuring an event by specifying the wrong type of the event args.
    /// </summary>
    public class EventArgsTypeMismatchException : FluentEventsException
    {
        internal EventArgsTypeMismatchException()
            : base("The specified event args type is different from the event args type of the event being selected.")
        {
        }
    }
}