namespace FluentEvents.Azure.ServiceBus.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the configuration connection string property is null.
    /// </summary>
    public class ConnectionStringIsNullException : FluentEventsServiceBusException
    {
        internal ConnectionStringIsNullException()
            : base("The connection string is null")
        {
        }
    }
}