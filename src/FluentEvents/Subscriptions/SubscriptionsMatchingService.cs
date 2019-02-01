using System.Collections.Generic;
using System.Linq;
using FluentEvents.Utils;

namespace FluentEvents.Subscriptions
{
    public class SubscriptionsMatchingService : ISubscriptionsMatchingService
    {
        public IEnumerable<Subscription> GetMatchingSubscriptionsForSender(
            IEnumerable<Subscription> subscriptions,
            object sender
        )
        {
            var types = sender.GetType().GetBaseTypesInclusive();

            return subscriptions.Where(x => types.Any(y => y == x.SourceType));
        }
    }
}