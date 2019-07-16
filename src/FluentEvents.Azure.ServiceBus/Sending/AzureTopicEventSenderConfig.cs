namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class AzureTopicEventSenderConfig
    {
        /// <summary>
        ///     An Azure Service Bus topic connection string for sending messages. 
        /// </summary>
        public string SendConnectionString { get; set; }
    }
}