using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal class GlobalSubscriptionsService : IGlobalSubscriptionsService
    {
        private readonly ConcurrentDictionary<Subscription, bool> _globalSubscriptions;
        private readonly ConcurrentQueue<ISubscriptionCreationTask> _subscriptionCreationTasks;
        private readonly IRootAppServiceProvider _rootAppServiceProvider;

        
        public GlobalSubscriptionsService(IRootAppServiceProvider rootAppServiceProvider)
        {
            _globalSubscriptions = new ConcurrentDictionary<Subscription, bool>();
            _subscriptionCreationTasks = new ConcurrentQueue<ISubscriptionCreationTask>();
            _rootAppServiceProvider = rootAppServiceProvider;
        }
        
        public void AddGlobalServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class
        {
            _subscriptionCreationTasks.Enqueue(new ServiceHandlerSubscriptionCreationTask<TService, TEvent>(isOptional));
        }

        public IEnumerable<Subscription> GetGlobalSubscriptions()
        {
            while (_subscriptionCreationTasks.TryDequeue(out var subscriptionCreationTask))
                foreach (var subscription in subscriptionCreationTask.CreateSubscriptions(_rootAppServiceProvider))
                    _globalSubscriptions.TryAdd(subscription, true);

            return _globalSubscriptions.Keys;
        }
    }
}
