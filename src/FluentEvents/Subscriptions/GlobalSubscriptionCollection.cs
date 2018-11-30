using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public class GlobalSubscriptionCollection : IGlobalSubscriptionCollection
    {
        private readonly ConcurrentDictionary<Subscription, bool> m_GlobalScopeSubscriptions;
        private readonly ConcurrentQueue<SubscriptionCreationTask> m_SubscriptionCreationTasks;
        private readonly ISubscriptionsFactory m_SubscriptionsFactory;
        private readonly IServiceProvider m_ServiceProvider;

        public GlobalSubscriptionCollection(ISubscriptionsFactory subscriptionsFactory, IServiceProvider serviceProvider)
        {
            m_GlobalScopeSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            m_SubscriptionCreationTasks = new ConcurrentQueue<SubscriptionCreationTask>();
            m_SubscriptionsFactory = subscriptionsFactory;
            m_ServiceProvider = serviceProvider;
        }
        
        public Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var subscription = m_SubscriptionsFactory.CreateSubscription(subscriptionAction);
            m_GlobalScopeSubscriptions.TryAdd(subscription, true);
            return subscription;
        }

        public void AddGlobalScopeSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
        {
            m_SubscriptionCreationTasks.Enqueue(new SubscriptionCreationTask<TService,TSource>(subscriptionAction, m_SubscriptionsFactory));
        }

        public void RemoveGlobalScopeSubscription(Subscription subscription) => m_GlobalScopeSubscriptions.TryRemove(subscription, out _);

        public IEnumerable<Subscription> GetGlobalScopeSubscriptions()
        {
            while (m_SubscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                m_GlobalScopeSubscriptions.TryAdd(subscriptionCreationTask.CreateSubscription(m_ServiceProvider), true);

            return m_GlobalScopeSubscriptions.Keys;
        }
    }
}
