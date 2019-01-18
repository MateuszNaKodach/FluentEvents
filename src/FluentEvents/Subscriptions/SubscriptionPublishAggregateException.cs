using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentEvents.Subscriptions
{
    internal class SubscriptionPublishAggregateException : AggregateException
    {
        internal SubscriptionPublishAggregateException(IEnumerable<TargetInvocationException> targetInvocationExceptions) 
            : base(targetInvocationExceptions.Select(x => x.InnerException))
        {
        }
    }
}