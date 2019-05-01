using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class GlobalSubscriptionCollection : IGlobalSubscriptionCollection
    {
        private readonly ConcurrentDictionary<Subscription, bool> _globalScopeSubscriptions;
        private readonly ConcurrentQueue<ISubscriptionCreationTask> _subscriptionCreationTasks;
        private readonly ISubscriptionsFactory _subscriptionsFactory;
        private readonly IAppServiceProvider _appServiceProvider;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public GlobalSubscriptionCollection(ISubscriptionsFactory subscriptionsFactory, IAppServiceProvider appServiceProvider)
        {
            _globalScopeSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            _subscriptionCreationTasks = new ConcurrentQueue<ISubscriptionCreationTask>();
            _subscriptionsFactory = subscriptionsFactory;
            _appServiceProvider = appServiceProvider;
        }

        /// <inheritdoc />
        public Subscription AddGlobalScopeSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var subscription = _subscriptionsFactory.CreateSubscription(subscriptionAction);
            _globalScopeSubscriptions.TryAdd(subscription, true);
            return subscription;
        }

        /// <inheritdoc />
        public void AddGlobalScopeServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
        {
            _subscriptionCreationTasks.Enqueue(
                new SubscriptionCreationTask<TService, TSource>(subscriptionAction, _subscriptionsFactory)
            );
        }

        /// <inheritdoc />
        public void RemoveGlobalScopeSubscription(ISubscriptionsCancellationToken subscription) 
            => _globalScopeSubscriptions.TryRemove((Subscription)subscription, out _);

        /// <inheritdoc />
        public IEnumerable<Subscription> GetGlobalScopeSubscriptions()
        {
            while (_subscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                _globalScopeSubscriptions.TryAdd(subscriptionCreationTask.CreateSubscription(_appServiceProvider), true);

            return _globalScopeSubscriptions.Keys;
        }
    }
}
