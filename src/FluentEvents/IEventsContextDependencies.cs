using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents
{
    internal interface IEventsContextDependencies
    {
        IScopedSubscriptionsService ScopedSubscriptionsService { get; }
        IGlobalSubscriptionCollection GlobalSubscriptionCollection { get; }
        IEventReceiversService EventReceiversService { get; }
        ITypesResolutionService TypesResolutionService { get; }
        ISourceModelsService SourceModelsService { get; }
        IRoutingService RoutingService { get; }
        IAttachingService AttachingService { get; }
        IEventsQueuesService EventsQueuesService { get; }
    }
}