using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    public class EventsScope
    {
        internal IServiceProvider ServiceProvider { get; }

        private readonly IEnumerable<IEventsContext> m_EventsContexts;
        private readonly IEventsQueuesService m_EventsQueuesService;

        private readonly object m_SyncSubscriptions = new object();
        private IEnumerable<Subscription> m_Subscriptions;

        internal EventsScope()
        {
        }

        public EventsScope(
            IEnumerable<IEventsContext> eventsContexts,
            IServiceProvider serviceProvider,
            IEventsQueuesService eventsQueuesService
        )
        {
            ServiceProvider = serviceProvider;

            m_EventsContexts = eventsContexts;
            m_EventsQueuesService = eventsQueuesService;
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
                            scopedSubscriptionsService.CreateScopedSubscriptionsForServices(ServiceProvider)
                        );
                    }

                    m_Subscriptions = subscriptions;
                }

                return m_Subscriptions;
            }
        }

        internal virtual Task ProcessQueuedEventsAsync(IEventsContext eventsContext, string queueName) 
            => m_EventsQueuesService.ProcessQueuedEventsAsync(this, eventsContext, queueName);

        internal virtual void DiscardQueuedEvents(IEventsContext eventsContext, string queueName) 
            => m_EventsQueuesService.DiscardQueuedEvents(eventsContext, queueName);

        internal virtual void EnqueueEvent(PipelineEvent pipelineEvent, IPipeline pipeline) 
            => m_EventsQueuesService.EnqueueEvent(pipelineEvent, pipeline);
    }
}
