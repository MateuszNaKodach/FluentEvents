
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class TopicEventSenderConfig : IValidableConfig
    {
        private string m_ConnectionString;

        /// <summary>
        ///     An Azure Service Bus topic connection string for sending messages. 
        /// </summary>
        public string ConnectionString
        {
            get => m_ConnectionString;
            set => m_ConnectionString = ConnectionStringValidator.ValidateOrThrow(value);
        }

        void IValidableConfig.Validate()
        {
            if (ConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}