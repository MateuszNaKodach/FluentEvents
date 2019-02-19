using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IScopedSubscriptionsService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void ConfigureScopedServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
            where TService : class
            where TSource : class;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<Subscription> SubscribeServices(IServiceProvider serviceProvider);
    }
}