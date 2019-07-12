using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    internal class SubscriptionPublishAggregateException : AggregateException
    {
        internal SubscriptionPublishAggregateException(IEnumerable<Exception> targetInvocationExceptions) 
            : base(targetInvocationExceptions)
        {
        }
    }
}