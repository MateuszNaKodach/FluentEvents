using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class MultipleEventsContextsTest
    {

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext1(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) 
                : base(options, rootAppServiceProvider)
            {
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext2(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }

        private class TestEventsContext3 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext3(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
