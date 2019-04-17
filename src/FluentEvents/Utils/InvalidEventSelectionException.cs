using System;

namespace FluentEvents.Utils
{
    /// <summary>
    ///     An exception thrown when using the dynamic object in ways that differ from using it as an event handler. 
    /// </summary>
    public class InvalidEventSelectionException : FluentEventsException
    {
        internal InvalidEventSelectionException(Exception innerException) : base("", innerException)
        {
        }
    }
}