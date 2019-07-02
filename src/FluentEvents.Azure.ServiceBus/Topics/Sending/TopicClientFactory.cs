using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class TopicClientFactory : ITopicClientFactory
    {
        public ITopicClient GetNew(string connectionString)
        {
            return new TopicClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}
