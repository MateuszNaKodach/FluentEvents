using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IGlobalSubscriptionCollection
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void AddGlobalScopeServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void RemoveGlobalScopeSubscription(ISubscriptionsCancellationToken subscription);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<Subscription> GetGlobalScopeSubscriptions();
    }
}