using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class UnknownEventSenderTest
    {
        private TestEventsContext _testEventsContext;
        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options => { });

            var serviceProvider = services.BuildServiceProvider();
            _testEventsContext = serviceProvider.GetService<TestEventsContext>();
            _eventsScope = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<EventsScope>();
        }

        [Test]
        public void WhenEventSenderIsNotRegistered_OnBuildingPipelines_ShouldThrow()
        {
            Assert.That(() =>
            {
                _testEventsContext.Attach(new TestEntity(), _eventsScope);
            }, Throws.TypeOf<EventTransmissionPluginIsNotConfiguredException>());
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
            {
                var pipelineBuilder = pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped();

                pipelineBuilder.ThenIsPublishedToGlobalSubscriptions(x => new PublishTransmissionConfiguration(typeof(object)));
            }

            public TestEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider)
                : base(options, rootAppServiceProvider)
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
