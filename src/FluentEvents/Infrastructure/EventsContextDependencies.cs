using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents.Infrastructure
{
    internal class EventsContextDependencies : IEventsContextDependencies
    {
        public IGlobalSubscriptionsService GlobalSubscriptionsService { get; }
        public IEventReceiversService EventReceiversService { get; }
        public IAttachingService AttachingService { get; }
        public IEventsQueuesService EventsQueuesService { get; }

        public EventsContextDependencies(
            IGlobalSubscriptionsService globalSubscriptionsService,
            IEventReceiversService eventReceiversService, 
            IAttachingService attachingService,
            IEventsQueuesService eventsQueuesService
        )
        {
            GlobalSubscriptionsService = globalSubscriptionsService;
            EventReceiversService = eventReceiversService;
            AttachingService = attachingService;
            EventsQueuesService = eventsQueuesService;
        }
    }
}
