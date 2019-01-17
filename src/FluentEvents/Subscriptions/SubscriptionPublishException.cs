using System.Reflection;

namespace FluentEvents.Subscriptions
{
    internal class SubscriptionPublishException : FluentEventsException
    {
        public SubscriptionPublishException()
        {
        }

        public SubscriptionPublishException(TargetInvocationException targetInvocationException) 
            : base(targetInvocationException.Message, targetInvocationException.InnerException)
        {
        }
    }
}