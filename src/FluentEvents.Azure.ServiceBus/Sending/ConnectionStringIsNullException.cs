namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventSenderConfig.ConnectionString" /> property is null.
    /// </summary>
    public class ConnectionStringIsNullException : FluentEventsServiceBusException
    {
        internal ConnectionStringIsNullException()
            : base("The connection string is null")
        {
        }
    }
}