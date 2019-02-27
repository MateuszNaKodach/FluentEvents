using System;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     An exception thrown when the application's <see cref="IServiceProvider"/> can't resolve a service
    ///     with an automatic subscription configured.
    /// </summary>
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