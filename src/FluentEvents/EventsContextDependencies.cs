using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents
{
    internal class EventsContextDependencies : IEventsContextDependencies
    {
        public IGlobalSubscriptionCollection GlobalSubscriptionCollection { get; }
        public IEventReceiversService EventReceiversService { get; }
        public IAttachingService AttachingService { get; }
        public IEventsQueuesService EventsQueuesService { get; }

        public EventsContextDependencies(
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            IEventReceiversService eventReceiversService, 
            IAttachingService attachingService,
            IEventsQueuesService eventsQueuesService
        )
        {
            GlobalSubscriptionCollection = globalSubscriptionCollection;
            EventReceiversService = eventReceiversService;
            AttachingService = attachingService;
            EventsQueuesService = eventsQueuesService;
        }
    }
}
