using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents
{
    internal class EventsContextDependencies : IEventsContextDependencies
    {
        public IScopedSubscriptionsService ScopedSubscriptionsService { get; }
        public IGlobalSubscriptionCollection GlobalSubscriptionCollection { get; }
        public IEventReceiversService EventReceiversService { get; }
        public ITypesResolutionService TypesResolutionService { get; }
        public ISourceModelsService SourceModelsService { get; }
        public IRoutingService RoutingService { get; }
        public IAttachingService AttachingService { get; }

        public EventsContextDependencies(
            IScopedSubscriptionsService scopedSubscriptionsService,
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            IEventReceiversService eventReceiversService, 
            ITypesResolutionService typesResolutionService,
            ISourceModelsService sourceModelsService, 
            IRoutingService routingService,
            IAttachingService attachingService
        )
        {
            ScopedSubscriptionsService = scopedSubscriptionsService;
            GlobalSubscriptionCollection = globalSubscriptionCollection;
            EventReceiversService = eventReceiversService;
            TypesResolutionService = typesResolutionService;
            SourceModelsService = sourceModelsService;
            RoutingService = routingService;
            AttachingService = attachingService;
        }
    }
}
