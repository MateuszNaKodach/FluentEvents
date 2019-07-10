using System;

namespace FluentEvents.Azure.ServiceBus.Topics.Subscribing
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the creation of an Azure Service Bus topic subscription fails.
    /// </summary>
    public class TopicSubscriptionCreationException : FluentEventsServiceBusException
    {
        /// <inheritdoc />
        /// <summary>
        ///     Creates a new instance of <see cref="TopicSubscriptionCreationException" />
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public TopicSubscriptionCreationException(Exception innerException) 
            : base("Failed to create a subscription to the Azure Service Bus topic", innerException)
        {
        }
    }
}