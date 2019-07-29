using System;
using System.Reflection;

namespace FluentEvents.Subscriptions
{
    [Serializable]
    internal class SubscribedEventHandlerThrewException : FluentEventsException
    {
        public SubscribedEventHandlerThrewException(TargetInvocationException targetInvocationException) 
            : base("The event handler threw an exception.", targetInvocationException.InnerException)
        {
        }
    }
}