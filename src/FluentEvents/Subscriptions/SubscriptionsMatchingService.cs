using System.Collections.Generic;
using System.Linq;
using FluentEvents.Utils;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class SubscriptionsMatchingService : ISubscriptionsMatchingService
    {
        /// <inheritdoc />
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