using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <summary>
    ///     The <see cref="EventsScope"/> represents the scope where entities are attached and the events
    ///     are handled or queued.
    ///     An <see cref="EventsScope"/> should be treated as scoped and should be short-lived.
    /// </summary>
    public class EventsScope
    {
        private readonly IAppServiceProvider _scopedAppServiceProvider;
        private readonly IEnumerable<EventsContext> _eventsContexts;

        private readonly object _syncSubscriptions = new object();
        private IEnumerable<Subscription> _subscriptions;

        internal IEventsQueueCollection EventsQueues { get; }

        internal EventsScope()
        {
        }

        internal EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IAppServiceProvider scopedAppServiceProvider,
            IEventsQueueCollection eventsQueues
        ) 
        {
            _eventsContexts = eventsContexts;
            _scopedAppServiceProvider = scopedAppServiceProvider;
            EventsQueues = eventsQueues;
        }

        /// <param name="eventsContexts">A list of the <see cref="EventsContext"/>s in the current scope.</param>
        /// <param name="scopedAppServiceProvider">The application service provider.</param>
        public EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IServiceProvider scopedAppServiceProvider
        ) : this(eventsContexts, new AppServiceProvider(scopedAppServiceProvider), new EventsQueueCollection())
        {
        }

        internal virtual IEnumerable<Subscription> GetSubscriptions()
        {
            lock (_syncSubscriptions)
            {
                if (_subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    foreach (var eventsContext in _eventsContexts)
                    {
                        var scopedSubscriptionsService = eventsContext
                            .Get<IServiceProvider>()
                            .GetRequiredService<IScopedSubscriptionsService>();

                        subscriptions.AddRange(
                            scopedSubscriptionsService.SubscribeServices(_scopedAppServiceProvider)
                        );
                    }

                    _subscriptions = subscriptions;
                }

                return _subscriptions;
            }
        }
    }
}
