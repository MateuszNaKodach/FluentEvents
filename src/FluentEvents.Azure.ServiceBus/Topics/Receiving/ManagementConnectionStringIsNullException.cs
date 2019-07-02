namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when the <see cref="AzureServiceBusTopicEventReceiverConfig.ManagementConnectionString" /> property is null.
    /// </summary>
    public class ManagementConnectionStringIsNullException : FluentEventsServiceBusException
    {
        internal ManagementConnectionStringIsNullException()
            : base("The management connection string is null.")
        {
        }
    }
}