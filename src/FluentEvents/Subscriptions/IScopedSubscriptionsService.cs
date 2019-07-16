using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

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
        void ConfigureScopedServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IEventHandler<TEvent>
            where TEvent : class;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<Subscription> SubscribeServices(IScopedAppServiceProvider scopedAppServiceProvider);
    }
}