using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Queues
{
    internal class EventsQueuesService : IEventsQueuesService
    {
        private readonly IEventsContext _eventsContext;
        private readonly IEventsQueueNamesService _eventsQueueNamesService;

        public EventsQueuesService(IEventsContext eventsContext, IEventsQueueNamesService eventsQueueNamesService)
        {
            _eventsContext = eventsContext;
            _eventsQueueNamesService = eventsQueueNamesService;
        }

        public async Task ProcessQueuedEventsAsync(IEventsScope eventsScope, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            if (queueName != null)
            {
                if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                    throw new EventsQueueNotFoundException();

                var eventsQueue = eventsScope.GetQueuesFeature().GetOrAddEventsQueue(_eventsContext, queueName);
                await ProcessQueueAsync(eventsQueue).ConfigureAwait(false);
            }
            else
            {
                foreach (var eventsQueue in eventsScope.GetQueuesFeature().GetEventsQueues(_eventsContext))
                    await ProcessQueueAsync(eventsQueue).ConfigureAwait(false);
            }
        }

        private async Task ProcessQueueAsync(IEventsQueue eventsQueue)
        {
            foreach (var queuedPipelineEvent in eventsQueue.DequeueAll())
                await queuedPipelineEvent.InvokeNextModule().ConfigureAwait(false);
        }

        public void DiscardQueuedEvents(IEventsScope eventsScope, string queueName)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));

            if (queueName != null)
            {
                if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                    throw new EventsQueueNotFoundException();

                var eventsQueue = eventsScope.GetQueuesFeature().GetOrAddEventsQueue(_eventsContext, queueName);
                eventsQueue.DiscardQueuedEvents();
            }
            else
            {
                foreach (var eventsQueue in eventsScope.GetQueuesFeature().GetEventsQueues(_eventsContext))
                    eventsQueue.DiscardQueuedEvents();
            }
        }

        public void EnqueueEvent(IEventsScope eventsScope, PipelineEvent pipelineEvent, string queueName, Func<Task> invokeNextModule)
        {
            if (eventsScope == null) throw new ArgumentNullException(nameof(eventsScope));
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            if (!_eventsQueueNamesService.IsQueueNameExisting(queueName))
                throw new EventsQueueNotFoundException();

            var queue = eventsScope.GetQueuesFeature().GetOrAddEventsQueue(_eventsContext, queueName);

            queue.Enqueue(new QueuedPipelineEvent(invokeNextModule));
        }
    }
}