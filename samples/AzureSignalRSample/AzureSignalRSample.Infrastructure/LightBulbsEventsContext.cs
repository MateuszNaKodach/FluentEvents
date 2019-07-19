using AzureSignalRSample.Domain;
using FluentEvents;
using FluentEvents.Azure.SignalR;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines.Queues;

namespace AzureSignalRSample.Infrastructure
{
    public class LightBulbsEventsContext : EventsContext
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

        public LightBulbsEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) : base(options, rootAppServiceProvider)
        {
        }
    }
}
