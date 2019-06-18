using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface ISubscriptionScanService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<SubscribedHandler> GetSubscribedHandlers(
            Type sourceType,
            IEnumerable<FieldInfo> fieldInfos,
            Action<object> subscriptionAction
        );
    }
}