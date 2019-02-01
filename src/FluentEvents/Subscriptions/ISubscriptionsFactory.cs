using System;

namespace FluentEvents.Subscriptions
{
    public interface ISubscriptionsFactory
    {
        Subscription CreateSubscription<TSource>(Action<TSource> subscriptionAction);
    }
}