using FluentEvents;
using FluentEvents.Config;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using WorkerSample.Domain;
using WorkerSample.Notifications;

namespace WorkerSample.Events
{
    public class AppEventsContext : EventsContext
    {
        public static string AfterSaveChangesQueueName { get; } = "AfterSaveChangesQueue";

        protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
        {
            subscriptionsBuilder
                .ServiceHandler<
                    ProductSubscriptionCancelledMailService,
                    ProductSubscription,
                    ProductSubscriptionCancelledEventArgs
                >()
                .HasGlobalSubscriptionTo((source, h) => source.Cancelled += h);
        }

        protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<ProductSubscription, ProductSubscriptionCancelledEventArgs>((source, h) => source.Cancelled += h)
                .IsWatched()
                // Publishing happens when the ProcessQueuedEventsAsync() method is called
                // by the override of DbContext.SaveChangesAsync().
                .ThenIsQueuedTo(AfterSaveChangesQueueName) 
                .ThenIsPublishedToGlobalSubscriptions();
        }
    }
}
