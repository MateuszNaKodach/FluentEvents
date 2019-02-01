using System;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionCreationTask
    {
        Subscription CreateSubscription(IServiceProvider serviceProvider);
    }
}