using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal interface ISubscriptionClientFactory
    {
        ISubscriptionClient GetNew(string receiveConnectionString, string subscriptionName);
    }
}