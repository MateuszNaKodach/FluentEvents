
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class AzureTopicEventSenderConfig : IValidableConfig
    {
        private string _connectionString;

        /// <summary>
        ///     An Azure Service Bus topic connection string for sending messages. 
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        void IValidableConfig.Validate()
        {
            if (ConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}