using System;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when starting the Azure Service Bus topic event receiver.
    /// </summary>
    public class TopicEventReceiverStartException : FluentEventsServiceBusException
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="TopicEventReceiverStartException"/>.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public TopicEventReceiverStartException(Exception innerException) : base(null, innerException)
        {
        }
    }
}