using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    public class EventsQueuesService : IEventsQueuesService
    {
        private readonly IEventsQueuesFactory m_EventsQueuesFactory;
        private readonly ConcurrentDictionary<(IEventsContext, string), IEventsQueue> m_EventQueues;

        public EventsQueuesService(IEventsQueuesFactory eventsQueuesFactory)
        {
            m_EventsQueuesFactory = eventsQueuesFactory;
            m_EventQueues = new ConcurrentDictionary<(IEventsContext, string), IEventsQueue>();
        }

        public void CreateQueueIfNotExists(IEventsContext eventsContext, string queueName)
        {
            if (eventsContext == null) throw new ArgumentNullException(nameof(eventsContext));
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            m_EventQueues.GetOrAdd(
                (eventsContext, queueName), 
                x => m_EventsQueuesFactory.GetNew(queueName)
            );
        }

        public async Task ProcessQueuedEventsAsync(EventsScope eventsScope, IEventsContext eventsContext, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (eventsContext == null) throw new ArgumentNullException(nameof(eventsContext));

            if (queueName != null)
            {
                if (m_EventQueues.TryGetValue((eventsContext, queueName), out var eventsQueue))
                    await ProcessQueue(eventsScope, eventsQueue);
                else
                    throw new EventsQueueNotFoundException();
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

        public void DiscardQueuedEvents(IEventsContext eventsContext, string queueName)
        {
            if (eventsContext == null) throw new ArgumentNullException(nameof(eventsContext));

            if (queueName != null)
            {
                if (m_EventQueues.TryGetValue((eventsContext, queueName), out var eventsQueue))
                    eventsQueue.DiscardQueuedEvents();
                else
                    throw new EventsQueueNotFoundException();
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

            if (m_EventQueues.TryGetValue((pipeline.EventsContext, pipeline.QueueName), out var queue))
                queue.Enqueue(new QueuedPipelineEvent
                {
                    Pipeline = pipeline,
                    PipelineEvent = pipelineEvent
                });
            else
                throw new EventsQueueNotFoundException();
        }
    }
}