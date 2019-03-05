namespace FluentEvents.Azure.ServiceBus
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="TopicEventReceiverConfig.ReceiveConnectionString" /> property is null.
    /// </summary>
    public class ReceiveConnectionStringIsNullException : FluentEventsServiceBusException
    {
    }
}