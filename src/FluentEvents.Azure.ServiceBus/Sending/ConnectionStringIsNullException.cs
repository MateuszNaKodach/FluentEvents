namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="TopicEventSenderConfig.ConnectionString" /> property is null.
    /// </summary>
    public class ConnectionStringIsNullException : FluentEventsServiceBusException
    {
    }
}