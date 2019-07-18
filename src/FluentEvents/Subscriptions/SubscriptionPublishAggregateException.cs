using System;
using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     An exception that aggregates all exceptions thrown by the handlers of an event.
    /// </summary>
    public class SubscriptionPublishAggregateException : AggregateException
    {
        internal SubscriptionPublishAggregateException(IEnumerable<Exception> exceptions) 
            : base(exceptions)
        {
        }
    }
}