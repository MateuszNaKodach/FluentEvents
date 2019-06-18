using System;

namespace FluentEvents.Config
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

        /// <summary>
        ///     Creates a new instance of <see cref="InvalidEventSelectionException"/>.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        protected InvalidEventSelectionException(string message) : base(message)
        {
            
        }
    }
}