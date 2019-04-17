namespace FluentEvents.Azure.ServiceBus.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureTopicEventReceiverConfig.ManagementConnectionString" /> property is null.
    /// </summary>
    public class ManagementConnectionStringIsNullException : FluentEventsServiceBusException
    {
        internal ManagementConnectionStringIsNullException()
            : base("The management connection string is null.")
        {
        }
    }
}