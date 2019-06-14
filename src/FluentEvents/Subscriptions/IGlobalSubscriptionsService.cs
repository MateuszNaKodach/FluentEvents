using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IGlobalSubscriptionsService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        Subscription AddGlobalSubscription<TSource>(Action<TSource> subscriptionAction);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void AddGlobalServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void AddGlobalServiceHandlerSubscription<TService, TSource, TEventArgs>(string eventName)
            where TService : class, IEventHandler<TSource, TEventArgs>
            where TSource : class
            where TEventArgs : class;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void RemoveGlobalSubscription(Subscription subscription);

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<Subscription> GetGlobalSubscriptions();
    }
}