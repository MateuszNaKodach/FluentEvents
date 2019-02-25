using System;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionCreationTask
    {
        Subscription CreateSubscription(IAppServiceProvider appServiceProvider);
    }
}