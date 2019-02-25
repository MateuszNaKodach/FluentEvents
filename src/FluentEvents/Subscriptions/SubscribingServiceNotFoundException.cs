using System;

namespace FluentEvents.Subscriptions
{
    public class SubscribingServiceNotFoundException : FluentEventsException
    {
        /// <summary>
        ///     The type of the service that wasn't found.
        /// </summary>
        public Type ServiceType { get; }

        internal SubscribingServiceNotFoundException(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}