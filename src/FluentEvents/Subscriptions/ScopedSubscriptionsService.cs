using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal class ScopedSubscriptionsService : IScopedSubscriptionsService
    {
        private readonly ISubscriptionsFactory m_SubscriptionsFactory;
        private readonly ConcurrentDictionary<ISubscriptionCreationTask, bool> m_ScopedSubscriptionCreationTasks;

        public ScopedSubscriptionsService(ISubscriptionsFactory subscriptionsFactory)
        {
            m_SubscriptionsFactory = subscriptionsFactory;
            m_ScopedSubscriptionCreationTasks = new ConcurrentDictionary<ISubscriptionCreationTask, bool>();
        }

        public void ConfigureScopedServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
            where TService : class
            where TSource : class
        {
            var serviceSubscriptionTask = new SubscriptionCreationTask<TService, TSource>(
                subscriptionAction,
                m_SubscriptionsFactory
            );

            m_ScopedSubscriptionCreationTasks.TryAdd(serviceSubscriptionTask, true);
        }

        public IEnumerable<Subscription> SubscribeServices(IAppServiceProvider appServiceProvider)
        {
            return m_ScopedSubscriptionCreationTasks.Keys
                .Select(subscriptionCreationTask => subscriptionCreationTask.CreateSubscription(appServiceProvider))
                .ToList();
        }
    }
}
