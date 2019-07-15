using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class GlobalSubscriptionsService : IGlobalSubscriptionsService
    {
        private readonly ConcurrentDictionary<Subscription, bool> _globalSubscriptions;
        private readonly ConcurrentQueue<ISubscriptionCreationTask> _subscriptionCreationTasks;
        private readonly IAppServiceProvider _appServiceProvider;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public GlobalSubscriptionsService(IAppServiceProvider appServiceProvider)
        {
            _globalSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            _subscriptionCreationTasks = new ConcurrentQueue<ISubscriptionCreationTask>();
            _appServiceProvider = appServiceProvider;
        }
        
        /// <inheritdoc />
        public void AddGlobalServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IEventHandler<TEvent>
            where TEvent : class
        {
            _subscriptionCreationTasks.Enqueue(new ServiceHandlerSubscriptionCreationTask<TService, TEvent>(isOptional));
        }

        /// <inheritdoc />
        public IEnumerable<Subscription> GetGlobalSubscriptions()
        {
            while (_subscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                foreach (var subscription in subscriptionCreationTask.CreateSubscriptions(_appServiceProvider))
                    _globalSubscriptions.TryAdd(subscription, true);

            return _globalSubscriptions.Keys;
        }
    }
}
