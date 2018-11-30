using System;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    public class ServiceSubscriptionsConfiguration<TService> where TService : class 
    {
        private readonly IScopedSubscriptionsService m_ScopedSubscriptionsService;
        private readonly IGlobalSubscriptionCollection m_GlobalSubscriptionCollection;

        public ServiceSubscriptionsConfiguration(
            IScopedSubscriptionsService scopedSubscriptionsService,
            IGlobalSubscriptionCollection globalSubscriptionCollection
        )
        {
            m_ScopedSubscriptionsService = scopedSubscriptionsService;
            m_GlobalSubscriptionCollection = globalSubscriptionCollection;
        }

        public ServiceSubscriptionsConfiguration<TService> HasScopedSubscription<TSource>(
            Action<TService, TSource> subscriptionCallback
        )
            where TSource : class
        {
            m_ScopedSubscriptionsService.ConfigureScopedServiceSubscription(subscriptionCallback);
            return this;
        }

        public ServiceSubscriptionsConfiguration<TService> HasGlobalSubscription<TSource>(
            Action<TService, TSource> subscriptionCallback
        )
            where TSource : class
        {
            m_GlobalSubscriptionCollection.AddGlobalScopeSubscription(subscriptionCallback);
            return this;
        }
    }
}