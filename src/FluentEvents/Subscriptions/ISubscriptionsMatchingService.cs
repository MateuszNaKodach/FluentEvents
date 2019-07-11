using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface ISubscriptionsMatchingService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<Subscription> GetMatchingSubscriptionsForEvent(
            IEnumerable<Subscription> subscriptions, 
            object sender
        );
    }
}