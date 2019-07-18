using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class EventsScopeSubscriptionsFeature : IEventsScopeSubscriptionsFeature
    {
        private readonly IScopedAppServiceProvider _scopedAppServiceProvider;

        private readonly object _syncSubscriptions = new object();
        private IEnumerable<Subscription> _subscriptions;

        public EventsScopeSubscriptionsFeature(IScopedAppServiceProvider scopedAppServiceProvider)
        {
            _scopedAppServiceProvider = scopedAppServiceProvider;
        }

        public IEnumerable<Subscription> GetSubscriptions(IEventsContext eventsContext)
        {
            lock (_syncSubscriptions)
            {
                if (_subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    var scopedSubscriptionsService = eventsContext
                        .Get<IServiceProvider>()
                        .GetRequiredService<IScopedSubscriptionsService>();

                    subscriptions.AddRange(
                        scopedSubscriptionsService.SubscribeServices(_scopedAppServiceProvider)
                    );

                    _subscriptions = subscriptions;
                }

                return _subscriptions;
            }
        }
    }
}