using System;
using FluentEvents.Azure.ServiceBus.Sending;

namespace FluentEvents.Azure.ServiceBus
{
    /// <summary>
    ///     An exception thrown when value of the <see cref="AzureTopicEventSenderConfig.ConnectionString"/> property
    ///     is invalid.
    /// </summary>
    public class InvalidConnectionStringException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="InvalidConnectionStringException"/>
        /// </summary>
        public InvalidConnectionStringException()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="InvalidConnectionStringException"/>
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public InvalidConnectionStringException(Exception innerException) 
            : base(null, innerException)
        {
        }
    }
}