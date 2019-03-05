using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal interface ITopicClientFactory
    {
        ITopicClient GetNew(string connectionString);
    }
}