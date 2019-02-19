using System;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface for configuring a service.
    /// </summary>
    public class ServiceConfigurator<TService> where TService : class 
    {
        private readonly IScopedSubscriptionsService m_ScopedSubscriptionsService;
        private readonly IGlobalSubscriptionCollection m_GlobalSubscriptionCollection;

        internal ServiceConfigurator(
            IScopedSubscriptionsService scopedSubscriptionsService,
            IGlobalSubscriptionCollection globalSubscriptionCollection
        )
        {
            m_ScopedSubscriptionsService = scopedSubscriptionsService;
            m_GlobalSubscriptionCollection = globalSubscriptionCollection;
        }

        /// <summary>
        /// Subscribes a service to scoped events the first time a matching event is published. 
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <param name="subscriptionCallback">
        /// The method that will be called to make the subscriptions to the source's events.
        /// </param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceConfigurator<TService> HasScopedSubscription<TSource>(
            Action<TService, TSource> subscriptionCallback
        )
            where TSource : class
        {
            m_ScopedSubscriptionsService.ConfigureScopedServiceSubscription(subscriptionCallback);
            return this;
        }

        /// <summary>
        /// Subscribes a service to global events when the EventsContext is initialized. 
        /// </summary>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <param name="subscriptionCallback">
        /// The method that will be called to make the subscriptions to the source's events.
        /// </param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceConfigurator<TService> HasGlobalSubscription<TSource>(
            Action<TService, TSource> subscriptionCallback
        )
            where TSource : class
        {
            m_GlobalSubscriptionCollection.AddGlobalScopeServiceSubscription(subscriptionCallback);
            return this;
        }
    }
}