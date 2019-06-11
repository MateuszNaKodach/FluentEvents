using FluentEvents.Config;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class MultipleEventsContextsTest
    {
        [Test]
        public void ServiceProviderShouldHaveOnlyOneEventsScope()
        {
            var services = new ServiceCollection();
            services.AddEventsContext<TestEventsContext1>(options => { });
            services.AddEventsContext<TestEventsContext2>(options => { });
            services.AddEventsContext<TestEventsContext3>(options => { });

            var serviceProvider = services.BuildServiceProvider();

            var eventsScope = serviceProvider.GetService<EventsScope>();
            var eventsScopes = serviceProvider.GetServices<EventsScope>();

            Assert.That(eventsScope, Is.Not.Null);
            Assert.That(eventsScopes, Has.One.Items);
        }

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }
        }
        private class TestEventsContext3 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }
        }
    }
}
