using System;

namespace FluentEvents.Utils
{
    /// <summary>
    ///     An exception thrown when using the dynamic object in ways that differ from using it as an event handler. 
    /// </summary>
    public class InvalidEventSelectionException : FluentEventsException
    {
        internal InvalidEventSelectionException(Exception innerException) 
            : base("The event selection action is invalid." +
                   " The provided dynamic object can only be used as an event handler", innerException)
        {
        }
    }
}