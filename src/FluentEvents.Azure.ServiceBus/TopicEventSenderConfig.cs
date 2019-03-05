
namespace FluentEvents.Azure.ServiceBus
{
    /// <summary>
    ///     The configuration for the Azure Service Bus topic events sender.
    /// </summary>
    public class TopicEventSenderConfig
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
    }
}