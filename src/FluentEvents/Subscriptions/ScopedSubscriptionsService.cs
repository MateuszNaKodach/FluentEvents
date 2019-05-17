using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ScopedSubscriptionsService : IScopedSubscriptionsService
    {
        private readonly ISubscriptionsFactory _subscriptionsFactory;
        private readonly ConcurrentDictionary<ISubscriptionCreationTask, bool> _scopedSubscriptionCreationTasks;

        public ScopedSubscriptionsService(ISubscriptionsFactory subscriptionsFactory)
        {
            _subscriptionsFactory = subscriptionsFactory;
            _scopedSubscriptionCreationTasks = new ConcurrentDictionary<ISubscriptionCreationTask, bool>();
        }

        public void ConfigureScopedServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
            where TService : class
            where TSource : class
        {
            var serviceSubscriptionTask = new SubscriptionCreationTask<TService, TSource>(
                subscriptionAction,
                _subscriptionsFactory
            );

            _scopedSubscriptionCreationTasks.TryAdd(serviceSubscriptionTask, true);
        }

        public IEnumerable<Subscription> SubscribeServices(IAppServiceProvider scopedAppServiceProvider)
        {
            return _scopedSubscriptionCreationTasks.Keys
                .Select(subscriptionCreationTask => subscriptionCreationTask.CreateSubscription(scopedAppServiceProvider))
                .ToList();
        }
    }
}
