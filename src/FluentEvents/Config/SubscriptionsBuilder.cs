using System;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    public class SubscriptionsBuilder : BuilderBase
    {
        private readonly IGlobalSubscriptionCollection m_GlobalSubscriptionCollection;
        private readonly IScopedSubscriptionsService m_ScopedSubscriptionsService;

        public SubscriptionsBuilder(
            EventsContext eventsContext,
            IServiceProvider serviceProvider,
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            IScopedSubscriptionsService scopedSubscriptionsService
        )
            : base(eventsContext, serviceProvider)
        {
            m_GlobalSubscriptionCollection = globalSubscriptionCollection;
            m_ScopedSubscriptionsService = scopedSubscriptionsService;
        }

        /// <summary>
        /// Returns an object that can be used to configure subscriptions for a service.
        /// configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The configuration object for the specified service.</returns>
        public ServiceConfigurator<TService> Service<TService>()
            where TService : class
        {
            return new ServiceConfigurator<TService>(
                m_ScopedSubscriptionsService,
                m_GlobalSubscriptionCollection
            );
        }
    }
}