using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class GlobalSubscriptionCollection : IGlobalSubscriptionCollection
    {
        private readonly ConcurrentDictionary<Subscription, bool> _globalSubscriptions;
        private readonly ConcurrentQueue<ISubscriptionCreationTask> _subscriptionCreationTasks;
        private readonly ISubscriptionsFactory _subscriptionsFactory;
        private readonly IAppServiceProvider _appServiceProvider;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public GlobalSubscriptionCollection(ISubscriptionsFactory subscriptionsFactory, IAppServiceProvider appServiceProvider)
        {
            _globalSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            _subscriptionCreationTasks = new ConcurrentQueue<ISubscriptionCreationTask>();
            _subscriptionsFactory = subscriptionsFactory;
            _appServiceProvider = appServiceProvider;
        }

        /// <inheritdoc />
        public Subscription AddGlobalSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var subscription = _subscriptionsFactory.CreateSubscription(subscriptionAction);
            _globalSubscriptions.TryAdd(subscription, true);
            return subscription;
        }

        /// <inheritdoc />
        public void AddGlobalServiceSubscription<TService, TSource>(Action<TService, TSource> subscriptionAction)
        {
            _subscriptionCreationTasks.Enqueue(
                new ServiceSubscriptionCreationTask<TService, TSource>(subscriptionAction, _subscriptionsFactory)
            );
        }

        /// <inheritdoc />
        public void AddGlobalServiceHandlerSubscription<TService, TSource, TEventArgs>(string eventName)
            where TService : class, IEventHandler<TSource, TEventArgs>
            where TSource : class
            where TEventArgs : class
        {
            _subscriptionCreationTasks.Enqueue(
                new ServiceHandlerSubscriptionCreationTask<TService, TSource, TEventArgs>(eventName, _subscriptionsFactory)
            );
        }

        /// <inheritdoc />
        public void RemoveGlobalSubscription(ISubscriptionsCancellationToken subscription) 
            => _globalSubscriptions.TryRemove((Subscription)subscription, out _);

        /// <inheritdoc />
        public IEnumerable<Subscription> GetGlobalSubscriptions()
        {
            while (_subscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                _globalSubscriptions.TryAdd(subscriptionCreationTask.CreateSubscription(_appServiceProvider), true);

            return _globalSubscriptions.Keys;
        }
    }
}
