using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Topics.Subscribing
{
    internal interface ISubscriptionClientFactory
    {
        ISubscriptionClient GetNew(string receiveConnectionString, string subscriptionName);
    }
}