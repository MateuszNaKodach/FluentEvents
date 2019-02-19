using System;
using System.Collections.Generic;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    public class EventsScope
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IEnumerable<EventsContext> m_EventsContexts;

        private readonly object m_SyncSubscriptions = new object();
        private IEnumerable<Subscription> m_Subscriptions;

        internal IEventsQueueCollection EventsQueues { get; }

        internal EventsScope()
        {
        }

        internal EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IServiceProvider serviceProvider,
            IEventsQueueCollection eventsQueues
        ) : this(eventsContexts, serviceProvider)
        {
            EventsQueues = eventsQueues;
        }

        public EventsScope(
            IEnumerable<EventsContext> eventsContexts,
            IServiceProvider serviceProvider
        ) 
        {
            m_EventsContexts = eventsContexts;
            m_ServiceProvider = serviceProvider;
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
                            scopedSubscriptionsService.SubscribeServices(m_ServiceProvider)
                        );
                    }

                    m_Subscriptions = subscriptions;
                }

                return m_Subscriptions;
            }
        }
    }
}
