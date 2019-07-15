using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal interface ISubscriptionCreationTask
    {
        IEnumerable<Subscription> CreateSubscriptions(IAppServiceProvider appServiceProvider);
    }
}