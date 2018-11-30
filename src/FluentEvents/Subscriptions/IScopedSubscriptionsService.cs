using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public interface IScopedSubscriptionsService
    {
        void ConfigureScopedServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
            where TService : class
            where TSource : class;

        IEnumerable<Subscription> CreateScopedSubscriptionsForServices(IServiceProvider serviceProvider);
    }
}