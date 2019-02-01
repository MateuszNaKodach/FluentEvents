using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    public interface ISubscriptionsMatchingService
    {
        IEnumerable<Subscription> GetMatchingSubscriptionsForSender(
            IEnumerable<Subscription> subscriptions, 
            object sender
        );
    }
}