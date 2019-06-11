using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class TopicSubscriptionsService : ITopicSubscriptionsService
    {
        public async Task CreateSubscriptionAsync(
            string managementConnectionString,
            string subscriptionName, 
            string topicPath,
            TimeSpan autoDeleteOnIdleTimeout,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var managementClient = new ManagementClient(managementConnectionString);

                await managementClient.CreateSubscriptionAsync(
                    new SubscriptionDescription(topicPath, subscriptionName)
                    {
                        AutoDeleteOnIdle = autoDeleteOnIdleTimeout
                    },
                    cancellationToken
                ).ConfigureAwait(false);
            }
            catch (ServiceBusException e)
            {
                throw new ServiceBusSubscriptionCreationException(e);
            }
        }
    }
}
