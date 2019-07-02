using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal interface ITopicClientFactory
    {
        ITopicClient GetNew(string connectionString);
    }
}