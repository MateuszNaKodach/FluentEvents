using FluentEvents;
using FluentEvents.Config;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using WorkerSample.DomainModel;

namespace WorkerSample.Events
{
    public class AppEventsContext : EventsContext
    {
        public static string AfterSaveChangesQueueName { get; } = "AfterSaveChangesQueue";

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
