using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;

namespace FluentEvents
{
    public class EventsScope
    {
        internal IServiceProvider ServiceProvider { get; }

        private readonly IEnumerable<IScopedSubscriptionsFactory> m_ScopedSubscriptionsFactories;
        private readonly IEventsQueuesService m_EventsQueuesService;

        private readonly object m_SyncSubscriptions = new object();
        private IEnumerable<Subscription> m_Subscriptions;

        internal EventsScope()
        {
        }

        public EventsScope(
            IEnumerable<IScopedSubscriptionsFactory> scopedSubscriptionsFactories,
            IServiceProvider serviceProvider,
            IEventsQueuesService eventsQueuesService
        )
        {
            ServiceProvider = serviceProvider;

            m_ScopedSubscriptionsFactories = scopedSubscriptionsFactories;
            m_EventsQueuesService = eventsQueuesService;
        }

        internal virtual IEnumerable<Subscription> GetSubscriptions()
        {
            lock (m_SyncSubscriptions)
            {
                if (m_Subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    foreach (var scopedSubscriptionsFactory in m_ScopedSubscriptionsFactories)
                        subscriptions.AddRange(
                            scopedSubscriptionsFactory.CreateScopedSubscriptionsForServices(ServiceProvider)
                        );

                    m_Subscriptions = subscriptions;
                }

                return m_Subscriptions;
            }
        }

        internal virtual Task ProcessQueuedEventsAsync(IEventsContext eventsContext, string queueName) 
            => m_EventsQueuesService.ProcessQueuedEventsAsync(this, eventsContext, queueName);

        internal virtual void DiscardQueuedEvents(IEventsContext eventsContext, string queueName) 
            => m_EventsQueuesService.DiscardQueuedEvents(eventsContext, queueName);

        internal virtual void EnqueueEvent(PipelineEvent pipelineEvent, Pipeline pipeline) 
            => m_EventsQueuesService.EnqueueEvent(pipelineEvent, pipeline);
    }
}
