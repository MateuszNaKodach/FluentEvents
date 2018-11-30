using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents
{
    internal class EventsContextDependencies
    {
        public IScopedSubscriptionsService ScopedSubscriptionsService { get; }
        public IGlobalSubscriptionCollection GlobalSubscriptionCollection { get; }
        public IEventReceiversService EventReceiversService { get; }
        public ITypesResolutionService TypesResolutionService { get; }
        public ISourceModelsService SourceModelsService { get; }
        public IEventsRoutingService EventsRoutingService { get; }
        public IAttachingService AttachingService { get; }

        public EventsContextDependencies(
            IScopedSubscriptionsService scopedSubscriptionsService,
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            IEventReceiversService eventReceiversService, 
            ITypesResolutionService typesResolutionService,
            ISourceModelsService sourceModelsService, 
            IEventsRoutingService eventsRoutingService,
            IAttachingService attachingService
        )
        {
            ScopedSubscriptionsService = scopedSubscriptionsService;
            GlobalSubscriptionCollection = globalSubscriptionCollection;
            EventReceiversService = eventReceiversService;
            TypesResolutionService = typesResolutionService;
            SourceModelsService = sourceModelsService;
            EventsRoutingService = eventsRoutingService;
            AttachingService = attachingService;
        }
    }
}
