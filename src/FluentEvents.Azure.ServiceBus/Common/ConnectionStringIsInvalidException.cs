using System;

namespace FluentEvents.Azure.ServiceBus.Common
{
    /// <summary>
    ///     An exception thrown when value a configured connection string is invalid.
    /// </summary>
    public class ConnectionStringIsInvalidException : FluentEventsServiceBusException
    {
        /// <summary>
        ///     Creates a new <see cref="ConnectionStringIsInvalidException"/>
        /// </summary>
        public ConnectionStringIsInvalidException() : base("Invalid connection string")
        {
        }

        /// <summary>
        ///     Creates a new <see cref="ConnectionStringIsInvalidException"/>
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public ConnectionStringIsInvalidException(Exception innerException) 
            : base("Invalid connection string", innerException)
        {
        }
    }
}