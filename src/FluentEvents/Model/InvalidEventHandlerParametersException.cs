namespace FluentEvents.Model
{
    /// <summary>
    ///     An exception that is thrown when trying to create an event on a <see cref="SourceModel"/>
    ///     that have an invalid delegate signature.
    /// </summary>
    public class InvalidEventHandlerParametersException : FluentEventsException
    {
        internal InvalidEventHandlerParametersException()
            : base("The signature of the event handler should have 2 parameters.")
        {
            
        }
    }
}