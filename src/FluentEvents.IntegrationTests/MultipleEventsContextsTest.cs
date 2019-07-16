using FluentEvents.Config;
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

            public TestEventsContext1(
                EventsContextOptions options,
                IAppServiceProvider appServiceProvider, 
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext2(
                EventsContextOptions options,
                IAppServiceProvider appServiceProvider,
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }

        private class TestEventsContext3 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
            }

            public TestEventsContext3(
                EventsContextOptions options,
                IAppServiceProvider appServiceProvider, 
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }
    }
}
