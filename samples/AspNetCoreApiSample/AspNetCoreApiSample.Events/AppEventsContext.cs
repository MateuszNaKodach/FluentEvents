using System.Collections.Generic;
using System.Text;
using AspNetCoreApiSample.Domain;
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
                // Publishing happens when the ProcessQueuedEventsAsync() method is called
                // by the override of DbContext.SaveChangesAsync().
                .ThenIsQueuedTo(AfterSaveChangesQueueName)
                // Subscribers can now subscribe to ContractEvents using async event handlers.
                .ThenIsProjected(
                    source => new ContractEvents
                    {
                        Id = source.Id
                    },
                    args => new ContractEvents.ContractTerminatedEventArgs
                    {
                        Reason = args.Reason
                    },
                    (source, h) => source.Terminated += h
                )
                .ThenIsPublishedToGlobalSubscriptions();
        }
    }
}
