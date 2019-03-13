using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class UnknownEventSenderTest
    {
        private TestEventsContext m_TestEventsContext;
        private EventsScope m_EventsScope;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options => { });

            var serviceProvider = services.BuildServiceProvider();
            m_TestEventsContext = serviceProvider.GetService<TestEventsContext>();
            m_EventsScope = serviceProvider.CreateScope().ServiceProvider.GetService<EventsScope>();
        }

        [Test]
        public void WhenEventSenderIsNotRegistered_OnBuildingPipelines_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_TestEventsContext.Attach(new TestEntity(), m_EventsScope);
            }, Throws.TypeOf<EventTransmissionPluginIsNotConfiguredException>());
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                var pipelineBuilder = pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsForwardedToPipeline();

                pipelineBuilder.ThenIsPublishedToGlobalSubscriptions(x => new PublishTransmissionConfiguration(typeof(object)));
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
