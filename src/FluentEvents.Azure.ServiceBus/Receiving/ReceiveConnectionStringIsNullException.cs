namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventReceiverConfig.ReceiveConnectionString" /> property is null.
    /// </summary>
    public class ReceiveConnectionStringIsNullException : FluentEventsServiceBusException
    {
        internal ReceiveConnectionStringIsNullException()
            : base("The receive connection string is null.")
        {
        }
    }
}