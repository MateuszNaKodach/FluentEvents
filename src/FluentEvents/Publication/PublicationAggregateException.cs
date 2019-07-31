using System;
using System.Collections.Generic;

namespace FluentEvents.Publication
{
    /// <summary>
    ///     An exception that aggregates all exceptions thrown by the handlers of an event.
    /// </summary>
    public class PublicationAggregateException : AggregateException
    {
        internal PublicationAggregateException(IEnumerable<Exception> exceptions) 
            : base(exceptions)
        {
        }
    }
}