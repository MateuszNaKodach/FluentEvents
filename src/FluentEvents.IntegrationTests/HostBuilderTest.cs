using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using FluentEvents.ServiceProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class HostBuilderTest
    {
        [Test]
        public async Task HostShouldBuildAndStartAndStop([Values] bool validateScopes)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEventsContext<TestEventsContext>(options => { });
                });

            hostBuilder.UseServiceProviderFactory(
                new DefaultServiceProviderFactory(new ServiceProviderOptions {ValidateScopes = validateScopes})
            );

            var host = hostBuilder.Build();

            await host.StartAsync();
            await host.StopAsync();
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
