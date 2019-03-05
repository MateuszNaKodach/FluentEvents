using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class TopicClientFactory : ITopicClientFactory
    {
        public ITopicClient GetNew(string connectionString)
        {
            if (connectionString == null)
                throw new ConnectionStringIsNullException();

            return new TopicClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}
