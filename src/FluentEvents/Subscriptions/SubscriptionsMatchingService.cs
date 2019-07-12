using System.Collections.Generic;
using System.Linq;
using FluentEvents.Utils;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class SubscriptionsMatchingService : ISubscriptionsMatchingService
    {
        /// <inheritdoc />
        public IEnumerable<Subscription> GetMatchingSubscriptionsForEvent(
            IEnumerable<Subscription> subscriptions,
            object source
        )
        {
            var types = source.GetType().GetBaseTypesAndInterfacesInclusive();

            return subscriptions.Where(x => types.Any(y => y == x.EventType));
        }
    }
}