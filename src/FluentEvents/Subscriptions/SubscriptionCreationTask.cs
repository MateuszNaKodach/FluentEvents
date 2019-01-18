using System;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal abstract class SubscriptionCreationTask
    {
        public abstract Subscription CreateSubscription(IServiceProvider serviceProvider);
    }

    internal class SubscriptionCreationTask<TService, TSource> : SubscriptionCreationTask
    {
        private readonly Action<TService, TSource> m_SubscriptionAction;
        private readonly ISubscriptionsFactory m_SubscriptionsFactory;

        public SubscriptionCreationTask(Action<TService, TSource> subscriptionAction,
            ISubscriptionsFactory subscriptionsFactory)
        {
            m_SubscriptionAction = subscriptionAction;
            m_SubscriptionsFactory = subscriptionsFactory;
        }

        public override Subscription CreateSubscription(IServiceProvider serviceProvider)
        {
            var service = (TService) serviceProvider.GetRequiredService(typeof(TService));
            return m_SubscriptionsFactory.CreateSubscription<TSource>(x => m_SubscriptionAction(service, x));
        }
    }
}