using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class HostBuilderTest
    {
        [Test]
        public async Task HostShouldBuildAndStartAndStop()
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEventsContext<TestEventsContext>(options => { });
                })
                .Build();

            await host.StartAsync();
            await host.StopAsync();
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(
                EventsContextsRoot eventsContextsRoot,
                EventsContextOptions options,
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(eventsContextsRoot, options, scopedAppServiceProvider)
            {
            }
        }
    }
}
