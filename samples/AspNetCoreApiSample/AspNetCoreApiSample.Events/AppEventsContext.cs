using System.Collections.Generic;
using System.Text;
using AspNetCoreApiSample.DomainModel;
using FluentEvents;
using FluentEvents.Config;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;

namespace AspNetCoreApiSample.Events
{
    public class AppEventsContext : EventsContext
    {
        public static string AfterSaveChangesQueueName { get; } = "AfterSaveChangesQueue";

        protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
        {
            pipelinesBuilder
                .Event<Contract, ContractTerminatedEventArgs>((source, h) => source.Terminated += h)
                .IsWatched()
                .ThenIsQueuedTo(AfterSaveChangesQueueName)
                .ThenIsProjected(
                    source => new ContractEvents
                    {
                        Id = source.Id
                    },
                    args => new ContractEvents.ContractTerminatedEventArgs
                    {
                        Reason = args.Reason
                    }
                )
                .ThenIsPublishedToGlobalSubscriptions();
        }
    }
}
