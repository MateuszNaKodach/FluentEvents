namespace FluentEvents.Azure.ServiceBus.Common
{
    /// <summary>
    ///     The configuration for the Azure Service Bus events receiver.
    /// </summary>
    public abstract class AzureServiceBusEventReceiverConfigBase
    {
        /// <summary>
        ///     Gets or sets the maximum number of concurrent calls to the callback the message pump should initiate.
        /// </summary>
        /// <remarks>The default value is 1.</remarks>
        public int MaxConcurrentMessages { get; set; } = 1;

        /// <summary>
        ///     A connection string that can be used to receive messages from a topic subscription.
        /// </summary>
        public string ReceiveConnectionString { get; set; }
    }
}
