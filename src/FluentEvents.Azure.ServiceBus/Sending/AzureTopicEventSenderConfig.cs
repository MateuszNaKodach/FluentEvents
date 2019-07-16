
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class AzureTopicEventSenderConfig : IValidableConfig
    {
        private string _sendConnectionString;

        /// <summary>
        ///     An Azure Service Bus topic connection string for sending messages. 
        /// </summary>
        public string SendConnectionString
        {
            get => _sendConnectionString;
            set => _sendConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        void IValidableConfig.Validate()
        {
            if (SendConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}