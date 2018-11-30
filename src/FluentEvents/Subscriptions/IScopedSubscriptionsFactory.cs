using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public interface IScopedSubscriptionsFactory
    {
        IEnumerable<Subscription> CreateScopedSubscriptionsForServices(IServiceProvider serviceProvider);
    }
}