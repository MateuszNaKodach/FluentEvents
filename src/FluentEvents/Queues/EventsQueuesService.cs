using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal class EventsQueuesService : IEventsQueuesService
    {
        private readonly EventsQueuesContext _eventsQueuesContext;
        private readonly IEventsQueueNamesService _eventsQueueNamesService;

        public EventsQueuesService(EventsQueuesContext eventsQueuesContext, IEventsQueueNamesService eventsQueueNamesService)
        {
            _eventsQueuesContext = eventsQueuesContext;
            _eventsQueueNamesService = eventsQueueNamesService;
        }

        public async Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            if (queueName != null)
            {
                if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                    throw new EventsQueueNotFoundException();

                var eventsQueue = eventsScope.EventsQueues.GetOrAddEventsQueue(_eventsQueuesContext, queueName);
                await ProcessQueue(eventsScope, eventsQueue).ConfigureAwait(false);
            }
            else
            {
                foreach (var eventsQueue in eventsScope.EventsQueues)
                    await ProcessQueue(eventsScope, eventsQueue).ConfigureAwait(false);
            }
        }

        private async Task ProcessQueue(EventsScope eventsScope, IEventsQueue eventsQueue)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (eventsQueue == null) throw new ArgumentNullException(nameof(eventsQueue));

            foreach (var queuedPipelineEvent in eventsQueue.DequeueAll())
                await queuedPipelineEvent.InvokeNextModule().ConfigureAwait(false);
        }

        public void DiscardQueuedEvents(EventsScope eventsScope, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            if (queueName != null)
            {
                if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                    throw new EventsQueueNotFoundException();

                var eventsQueue = eventsScope.EventsQueues.GetOrAddEventsQueue(_eventsQueuesContext, queueName);
                eventsQueue.DiscardQueuedEvents();
            }
            else
            {
                foreach (var eventsQueue in eventsScope.EventsQueues)
                    eventsQueue.DiscardQueuedEvents();
            }
        }

        public void EnqueueEvent(EventsScope eventsScope, PipelineEvent pipelineEvent, string queueName, Func<Task> invokeNextModule)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                throw new EventsQueueNotFoundException();

            var queue = eventsScope.EventsQueues.GetOrAddEventsQueue(_eventsQueuesContext, queueName);

            queue.Enqueue(new QueuedPipelineEvent
            {
                InvokeNextModule = invokeNextModule,
                PipelineEvent = pipelineEvent
            });
        }
    }
}