using FluentEvents.Configuration;
using FluentEvents.Pipelines.Publication;
using FluentEvents.ServiceProviders;

namespace FluentEvents.Benchmarks.Common
{
    public class ScopedSubscriptionsEventsContext : EventsContext
    {
        protected override void OnBuildingSubscriptions(ISubscriptionsBuilder subscriptionsBuilder)
        {
            subscriptionsBuilder
                .ServiceHandler<EventsHandlingService, ScopedEventRaised>()
                .HasScopedSubscription();
        }

        protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<ScopedEventRaised>()
                .IsPiped()
                .ThenIsPublishedToScopedSubscriptions();
        }

        public ScopedSubscriptionsEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
            : base(options, rootAppServiceProvider)
        {
        }
    }
}