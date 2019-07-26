using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class MultipleEventsContextsTest
    {

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext1(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) 
                : base(options, rootAppServiceProvider)
            {
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext2(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }

        private class TestEventsContext3 : EventsContext
        {
            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext3(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
