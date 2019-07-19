using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionCreationTask
    {
        /// <exception cref="SubscribingServiceNotFoundException">
        ///     The service provider cannot resolve the subscribing service.
        /// </exception>
        IEnumerable<Subscription> CreateSubscriptions(IServiceProvider appServiceProvider);
    }
}