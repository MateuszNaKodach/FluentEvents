using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class UnknownPipelineModuleTest
    {
        private TestEventsContext _testEventsContext;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options => { });

            var serviceProvider = services.BuildServiceProvider();
            _testEventsContext = serviceProvider.GetService<TestEventsContext>();
        }

        [Test]
        public void WhenPipelineModuleIsNotRegistered_Publishing_ShouldThrow()
        {
            Assert.That(() =>
            {
                TestUtils.AttachAndRaiseEvent(_testEventsContext);
            }, Throws.TypeOf<PipelineModuleNotFoundException>());
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                var pipelineBuilder = pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped();

                var pipeline = pipelineBuilder.Get<IPipeline>();

                pipeline.AddModule<TestPipelineModule, TestPipelineModuleConfig>(new TestPipelineModuleConfig());
            }

            public TestEventsContext(
                EventsContextOptions options,
                IAppServiceProvider appServiceProvider,
                IScopedAppServiceProvider scopedAppServiceProvider
            ) : base(options, appServiceProvider, scopedAppServiceProvider)
            {
            }
        }

        private class TestPipelineModule : IPipelineModule<TestPipelineModuleConfig>
        {
            public Task InvokeAsync(TestPipelineModuleConfig config, PipelineContext pipelineContext, NextModuleDelegate invokeNextModule) 
                => Task.CompletedTask;
        }

        private class TestPipelineModuleConfig
        {
        }
    }
}
