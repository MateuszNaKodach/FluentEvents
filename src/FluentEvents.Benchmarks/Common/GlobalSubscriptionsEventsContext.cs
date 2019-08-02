using FluentEvents.Configuration;
using FluentEvents.Pipelines.Publication;
using FluentEvents.ServiceProviders;

namespace FluentEvents.Benchmarks.Common
{
    public class GlobalSubscriptionsEventsContext : EventsContext
    {
        protected override void OnBuildingSubscriptions(ISubscriptionsBuilder subscriptionsBuilder)
        {
            subscriptionsBuilder
                .ServiceHandler<EventsHandlingService, ScopedEventRaised>()
                .HasGlobalSubscription();
        }

        protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<ScopedEventRaised>()
                .IsPiped()
                .ThenIsPublishedToGlobalSubscriptions();
        }

        public GlobalSubscriptionsEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
            : base(options, rootAppServiceProvider)
        {
        }
    }
}