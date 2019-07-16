using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Infrastructure
{
    internal class EventsScope : IEventsScope
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly IScopedAppServiceProvider _scopedAppServiceProvider;

        private readonly object _syncSubscriptions = new object();
        private IEnumerable<Subscription> _subscriptions;

        private readonly ConcurrentDictionary<string, IEventsQueue> _eventsQueues;

        internal EventsScope(IServiceProvider internalServiceProvider, IScopedAppServiceProvider scopedAppServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider;
            _scopedAppServiceProvider = scopedAppServiceProvider;
            _eventsQueues = new ConcurrentDictionary<string, IEventsQueue>();
        }

        public IEnumerable<Subscription> GetSubscriptions()
        {
            lock (_syncSubscriptions)
            {
                if (_subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    var scopedSubscriptionsService = _internalServiceProvider
                        .GetRequiredService<IScopedSubscriptionsService>();

                    subscriptions.AddRange(
                        scopedSubscriptionsService.SubscribeServices(_scopedAppServiceProvider)
                    );

                    _subscriptions = subscriptions;
                }

                return _subscriptions;
            }
        }

        public IEnumerable<IEventsQueue> GetEventsQueues()
        {
            return _eventsQueues.Values;
        }

        public IEventsQueue GetOrAddEventsQueue(string queueName)
        {
            return _eventsQueues.GetOrAdd(queueName, x => new EventsQueue(queueName));
        }
    }
}
