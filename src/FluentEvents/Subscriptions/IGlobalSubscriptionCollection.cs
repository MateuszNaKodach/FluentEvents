using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public interface IGlobalSubscriptionCollection
    {
        Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction);
        void AddGlobalScopeSubscription<TService, TSource>(Action<TService, TSource> subscriptionCallback);
        void RemoveGlobalScopeSubscription(Subscription subscription);
        IEnumerable<Subscription> GetGlobalScopeSubscriptions();
       
    }
}