using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionCreationTask
    {
        IEnumerable<Subscription> CreateSubscriptions(IServiceProvider appServiceProvider);
    }
}