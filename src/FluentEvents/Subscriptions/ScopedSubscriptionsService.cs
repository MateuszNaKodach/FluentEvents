using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    internal class ScopedSubscriptionsService : IScopedSubscriptionsService
    {
        private readonly ISubscriptionsFactory m_SubscriptionsFactory;
        private readonly ConcurrentDictionary<SubscriptionCreationTask, bool> m_ScopedSubscriptionCreationTasks;

        public ScopedSubscriptionsService(ISubscriptionsFactory subscriptionsFactory)
        {
            m_SubscriptionsFactory = subscriptionsFactory;
            m_ScopedSubscriptionCreationTasks = new ConcurrentDictionary<SubscriptionCreationTask, bool>();
        }

        public void ConfigureScopedServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
            where TService : class
            where TSource : class
        {
            var serviceSubscriptionTask =
                new SubscriptionCreationTask<TService, TSource>(subscriptionAction,
                    m_SubscriptionsFactory);

            m_ScopedSubscriptionCreationTasks.TryAdd(serviceSubscriptionTask, true);
        }

        public IEnumerable<Subscription> SubscribeServices(IServiceProvider serviceProvider)
        {
            var subscriptions = new List<Subscription>();

            foreach (var subscriptionCreationTask in m_ScopedSubscriptionCreationTasks.Keys)
                subscriptions.Add(subscriptionCreationTask.CreateSubscription(serviceProvider));

            return subscriptions;
        }
    }
}
