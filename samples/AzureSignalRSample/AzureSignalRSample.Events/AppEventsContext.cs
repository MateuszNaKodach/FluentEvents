using AzureSignalRSample.DomainModel;
using FluentEvents;
using FluentEvents.Azure.SignalR;
using FluentEvents.Config;
using FluentEvents.Pipelines.Queues;

namespace AzureSignalRSample.Events
{
    public class AppEventsContext : EventsContext
    {
        public static string AfterSaveChangesQueueName { get; } = "AfterSaveChangesQueue";

        protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<LightBulb, LightBulbPowerStatusChangedEventArgs>((source, h) => source.PowerStatusChanged += h)
                .IsWatched()
                .ThenIsQueuedTo("AfterSaveChangesQueue")
                .ThenIsSentToAllAzureSignalRUsers();
        }

    }
}
