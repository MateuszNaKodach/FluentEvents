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

        public ServiceSubscriptionsConfiguration<TService> Service<TService>()
            where TService : class
        {
            return new ServiceSubscriptionsConfiguration<TService>(
                m_ScopedSubscriptionsService,
                m_GlobalSubscriptionCollection
            );
        }
    }
}