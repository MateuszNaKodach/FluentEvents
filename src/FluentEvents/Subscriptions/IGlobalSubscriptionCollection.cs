using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public interface IGlobalSubscriptionCollection
    {
        Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction);
        void AddGlobalScopeServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction);
        void RemoveGlobalScopeSubscription(ISubscriptionsCancellationToken subscription);
        IEnumerable<Subscription> GetGlobalScopeSubscriptions();
    }
}