using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class SubscriptionClientFactory : ISubscriptionClientFactory
    {
        public ISubscriptionClient GetNew(string receiveConnectionString, string subscriptionName)
        {
            return new SubscriptionClient(
                new ServiceBusConnectionStringBuilder(receiveConnectionString),
                subscriptionName
            );
        }
    }
}
