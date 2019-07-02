using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEvents.Azure.ServiceBus.Topics.Subscribing
{
    internal interface ITopicSubscriptionsService
    {
        Task CreateSubscriptionAsync(
            string managementConnectionString,
            string subscriptionName,
            string topicPath,
            TimeSpan autoDeleteOnIdleTimeout,
            CancellationToken cancellationToken = default
        );
    }
}