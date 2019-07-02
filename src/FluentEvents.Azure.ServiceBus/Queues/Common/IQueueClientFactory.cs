using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Queues.Common
{
    internal interface IQueueClientFactory
    {
        IQueueClient GetNew(string connectionString);
    }
}
