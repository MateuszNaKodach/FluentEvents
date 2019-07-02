using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Queues.Common
{
    internal class QueueClientFactory : IQueueClientFactory
    {
        public IQueueClient GetNew(string connectionString)
        {
            return new QueueClient(new ServiceBusConnectionStringBuilder(connectionString));
        }
    }
}