using System;

namespace FluentEvents.Subscriptions
{
    public interface ISubscriptionsFactory
    {
        Subscription CreateSubscription<TSource>(Action<TSource> subscriptionAction);
        Subscription CreateSubscription(Type sourceType, Action<object> subscriptionAction);
    }
}