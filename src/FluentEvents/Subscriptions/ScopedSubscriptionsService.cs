using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal class ScopedSubscriptionsService : IScopedSubscriptionsService
    {
        private readonly ConcurrentDictionary<ISubscriptionCreationTask, bool> _scopedSubscriptionCreationTasks;

        public ScopedSubscriptionsService()
        {
            _scopedSubscriptionCreationTasks = new ConcurrentDictionary<ISubscriptionCreationTask, bool>();
        }

        public void ConfigureScopedServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IEventHandler<TEvent>
            where TEvent : class
        {
            var serviceSubscriptionTask = new ServiceHandlerSubscriptionCreationTask<TService, TEvent>(isOptional);

            _scopedSubscriptionCreationTasks.TryAdd(serviceSubscriptionTask, true);
        }

        public IEnumerable<Subscription> SubscribeServices(IScopedAppServiceProvider scopedAppServiceProvider)
        {
            return _scopedSubscriptionCreationTasks.Keys
                .SelectMany(subscriptionCreationTask => subscriptionCreationTask.CreateSubscriptions(scopedAppServiceProvider))
                .ToList();
        }
    }
}
