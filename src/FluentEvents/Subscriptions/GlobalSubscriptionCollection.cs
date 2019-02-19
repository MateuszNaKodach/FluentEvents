using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class GlobalSubscriptionCollection : IGlobalSubscriptionCollection
    {
        private readonly ConcurrentDictionary<Subscription, bool> m_GlobalScopeSubscriptions;
        private readonly ConcurrentQueue<ISubscriptionCreationTask> m_SubscriptionCreationTasks;
        private readonly ISubscriptionsFactory m_SubscriptionsFactory;
        private readonly IServiceProvider m_ServiceProvider;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public GlobalSubscriptionCollection(ISubscriptionsFactory subscriptionsFactory, IServiceProvider serviceProvider)
        {
            m_GlobalScopeSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            m_SubscriptionCreationTasks = new ConcurrentQueue<ISubscriptionCreationTask>();
            m_SubscriptionsFactory = subscriptionsFactory;
            m_ServiceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var subscription = m_SubscriptionsFactory.CreateSubscription(subscriptionAction);
            m_GlobalScopeSubscriptions.TryAdd(subscription, true);
            return subscription;
        }

        /// <inheritdoc />
        public void AddGlobalScopeServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
        {
            m_SubscriptionCreationTasks.Enqueue(
                new SubscriptionCreationTask<TService, TSource>(subscriptionAction, m_SubscriptionsFactory)
            );
        }

        /// <inheritdoc />
        public void RemoveGlobalScopeSubscription(ISubscriptionsCancellationToken subscription) 
            => m_GlobalScopeSubscriptions.TryRemove((Subscription)subscription, out _);

        /// <inheritdoc />
        public IEnumerable<Subscription> GetGlobalScopeSubscriptions()
        {
            while (m_SubscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                m_GlobalScopeSubscriptions.TryAdd(subscriptionCreationTask.CreateSubscription(m_ServiceProvider), true);

            return m_GlobalScopeSubscriptions.Keys;
        }
    }
}
