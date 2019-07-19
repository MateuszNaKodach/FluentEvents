using AzureSignalRSample.Domain;
using FluentEvents;
using FluentEvents.Azure.SignalR;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines.Queues;

namespace AzureSignalRSample.Events
{
    public class AppEventsContext : EventsContext
    {
        public static string AfterSaveChangesQueueName { get; } = "AfterSaveChangesQueue";

        protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<LightBulbPowerStatusChanged>()
                .IsPiped()
                .ThenIsQueuedTo(AfterSaveChangesQueueName)
                .ThenIsSentToAllAzureSignalRUsers("lightBulbHub");
        }

        public AppEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) : base(options, rootAppServiceProvider)
        {
        }
    }
}
