using System;
using System.Collections.Generic;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    public interface ISubscriptionScanService
    {
        IEnumerable<SubscribedHandler> GetSubscribedHandlers(SourceModel sourceModel, Action<object> subscriptionAction);
    }
}