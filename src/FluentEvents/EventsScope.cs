using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <summary>
    ///     The EventsScope represents the scope where entities are attached and the events
    ///     are handled or queued.
    ///     An EventsScope should be treated as scoped and should be short-lived.
    /// </summary>
    public class EventsScope
    {
        private readonly IAppServiceProvider m_AppServiceProvider;
        private readonly IEnumerable<EventsContext> m_EventsContexts;

        private readonly object m_SyncSubscriptions = new object();
        private IEnumerable<Subscription> m_Subscriptions;

        internal IEventsQueueCollection EventsQueues { get; }

        internal EventsScope()
        {
        }

        internal EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IServiceProvider appServiceProvider,
            IEventsQueueCollection eventsQueues
        ) : this(eventsContexts, appServiceProvider)
        {
            EventsQueues = eventsQueues;
        }

        /// <param name="eventsContexts">A list of the <see cref="EventsContext"/>s in the current scope.</param>
        /// <param name="appServiceProvider">The application service provider.</param>
        public EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IServiceProvider appServiceProvider
        ) 
        {
            m_EventsContexts = eventsContexts;
            m_AppServiceProvider = new AppServiceProvider(appServiceProvider);
            EventsQueues = new EventsQueueCollection();
        }

        internal virtual IEnumerable<Subscription> GetSubscriptions()
        {
            lock (m_SyncSubscriptions)
            {
                if (m_Subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    foreach (var eventsContext in m_EventsContexts)
                    {
                        var scopedSubscriptionsService = eventsContext
                            .Get<IServiceProvider>()
                            .GetRequiredService<IScopedSubscriptionsService>();

                        subscriptions.AddRange(
                            scopedSubscriptionsService.SubscribeServices(m_AppServiceProvider)
                        );
                    }

                    m_Subscriptions = subscriptions;
                }

                return m_Subscriptions;
            }
        }
    }
}
