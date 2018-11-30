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

        public async Task ProcessQueuedEventsAsync(EventsScope eventsScope, IEventsContext eventsContext, string queueName)
        {
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
            foreach (var queuedPipelineEvent in eventsQueue.DequeueAll())
                await queuedPipelineEvent.Pipeline.ProcessEventAsync(queuedPipelineEvent.PipelineEvent, eventsScope);
        }

        public void DiscardQueuedEvents(IEventsContext eventsContext, string queueName)
        {
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

        public void EnqueueEvent(PipelineEvent pipelineEvent, Pipeline pipeline)
        {
            var queue = m_EventQueues.GetOrAdd((pipeline.EventsContext, pipeline.QueueName), x => m_EventsQueuesFactory.GetNew(pipeline.QueueName));
            queue.Enqueue(new QueuedPipelineEvent
            {
                Pipeline = pipeline,
                PipelineEvent = pipelineEvent
            });
        }
    }
}