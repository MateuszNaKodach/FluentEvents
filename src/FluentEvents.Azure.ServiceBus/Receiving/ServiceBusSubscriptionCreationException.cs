using System;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <summary>
    ///     An exception thrown when the creation of an Azure Service Bus topic subscription fails.
    /// </summary>
    [Serializable]
    public class ServiceBusSubscriptionCreationException : FluentEventsServiceBusException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ServiceBusSubscriptionCreationException" />
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        internal ServiceBusSubscriptionCreationException(Exception innerException) 
            : base("Failed to create a subscription to the Azure Service Bus topic", innerException)
        {
        }
    }
}