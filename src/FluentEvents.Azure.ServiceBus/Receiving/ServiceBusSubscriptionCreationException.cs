using System;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the creation of an Azure Service Bus topic subscription fails.
    /// </summary>
    public class ServiceBusSubscriptionCreationException : FluentEventsServiceBusException
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a new instance of <see cref="ServiceBusSubscriptionCreationException" />
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public ServiceBusSubscriptionCreationException(Exception innerException) : base(null, innerException)
        {
        }
    }
}