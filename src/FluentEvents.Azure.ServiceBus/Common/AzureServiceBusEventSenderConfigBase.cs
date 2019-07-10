

namespace FluentEvents.Azure.ServiceBus.Common
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class AzureServiceBusEventSenderConfigBase
    {
        /// <summary>
        ///     An Azure Service Bus topic connection string for sending messages. 
        /// </summary>
        public string SendConnectionString { get; set; }
    }
}