using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentEvents.Azure.ServiceBus.Receiving
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