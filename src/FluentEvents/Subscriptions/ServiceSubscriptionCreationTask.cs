using System;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ServiceSubscriptionCreationTask<TService, TSource> : ISubscriptionCreationTask
    {
        private readonly Action<TService, TSource> _subscriptionAction;
        private readonly ISubscriptionsFactory _subscriptionsFactory;

        public ServiceSubscriptionCreationTask(
            Action<TService, TSource> subscriptionAction,
            ISubscriptionsFactory subscriptionsFactory
        )
        {
            _subscriptionAction = subscriptionAction;
            _subscriptionsFactory = subscriptionsFactory;
        }

        public virtual Subscription CreateSubscription(IAppServiceProvider appServiceProvider)
        {
            var service = appServiceProvider.GetService<TService>();
            if (service == null)
                throw new SubscribingServiceNotFoundException(typeof(TService));

            return _subscriptionsFactory.CreateSubscription<TSource>(x => _subscriptionAction(service, x));
        }
    }
}