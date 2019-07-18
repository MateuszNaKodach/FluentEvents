using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionsMatchingService
    {
        IEnumerable<Subscription> GetMatchingSubscriptionsForEvent(
            IEnumerable<Subscription> subscriptions, 
            object source
        );
    }
}