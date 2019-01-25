using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Queues
{
    public class EventsQueuesService : IEventsQueuesService
    {
        private readonly IDictionary<IInfrastructureEventsContext, IEventsQueueNamesService> m_EventsQueueNamesServices;
        private readonly IEventsQueuesFactory m_EventsQueuesFactory;
        private readonly ConcurrentDictionary<(IInfrastructureEventsContext, string), IEventsQueue> m_EventQueues;

        public EventsQueuesService(
            IEnumerable<IInfrastructureEventsContext> eventsContexts,
            IEventsQueuesFactory eventsQueuesFactory
        )
        {
            m_EventsQueueNamesServices = eventsContexts
                .ToDictionary(
                    x => x,
                    x => x.Get<IServiceProvider>().GetRequiredService<IEventsQueueNamesService>()
                );
            m_EventsQueuesFactory = eventsQueuesFactory;
            m_EventQueues = new ConcurrentDictionary<(IInfrastructureEventsContext, string), IEventsQueue>();
        }

        private bool IsQueueNameValid(IInfrastructureEventsContext eventsContext, string queueName)
        {
            return m_EventsQueueNamesServices.TryGetValue(eventsContext, out var eventsQueueNamesService)
                   && eventsQueueNamesService.IsQueueNameExisting(queueName);
        }

        public async Task ProcessQueuedEventsAsync(EventsScope eventsScope, IInfrastructureEventsContext eventsContext, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (eventsContext == null) throw new ArgumentNullException(nameof(eventsContext));

            if (queueName != null)
            {
                if (!IsQueueNameValid(eventsContext, queueName))
                    throw new EventsQueueNotFoundException();

                if (m_EventQueues.TryGetValue((eventsContext, queueName), out var eventsQueue))
                    await ProcessQueue(eventsScope, eventsQueue);
            }
            else
            {
                foreach (var eventsQueue in m_EventQueues.Values)
                    await ProcessQueue(eventsScope, eventsQueue);
            }
        }

        private async Task ProcessQueue(EventsScope eventsScope, IEventsQueue eventsQueue)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (eventsQueue == null) throw new ArgumentNullException(nameof(eventsQueue));

            foreach (var queuedPipelineEvent in eventsQueue.DequeueAll())
                await queuedPipelineEvent.Pipeline.ProcessEventAsync(queuedPipelineEvent.PipelineEvent, eventsScope);
        }

        public void DiscardQueuedEvents(IInfrastructureEventsContext eventsContext, string queueName)
        {
            if (eventsContext == null) throw new ArgumentNullException(nameof(eventsContext));

            if (queueName != null)
            {
                if (!IsQueueNameValid(eventsContext, queueName))
                    throw new EventsQueueNotFoundException();

                if (m_EventQueues.TryGetValue((eventsContext, queueName), out var eventsQueue))
                    eventsQueue.DiscardQueuedEvents();
            }
            else
            {
                foreach (var eventsQueue in m_EventQueues.Values)
                    eventsQueue.DiscardQueuedEvents();
            }
        }

        public void EnqueueEvent(PipelineEvent pipelineEvent, IPipeline pipeline)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

            if (!IsQueueNameValid(pipeline.EventsContext, pipeline.QueueName))
                throw new EventsQueueNotFoundException();

            var queue = m_EventQueues.GetOrAdd(
                (pipeline.EventsContext, pipeline.QueueName),
                x => m_EventsQueuesFactory.GetNew(pipeline.QueueName)
            );

            queue.Enqueue(new QueuedPipelineEvent
            {
                Pipeline = pipeline,
                PipelineEvent = pipelineEvent
            });
        }
    }
}