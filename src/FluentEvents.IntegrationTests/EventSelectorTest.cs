using System;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class EventSelectorTest
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
            _eventsScope = serviceProvider.CreateScope().ServiceProvider.GetService<EventsScope>();
        }

        [Test]
        public void WhenEventSenderIsNotRegistered_OnBuildingPipelines_ShouldThrow()
        {
            object receivedSender = null;
            TestEventArgs receivedEventArgs = null;
            _testEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    receivedSender = sender;
                    receivedEventArgs = args;
                };
            });

            TestUtils.AttachAndRaiseEvent(_testEventsContext, _eventsScope);

            TestUtils.AssertThatEventIsPublishedProperly(receivedSender, receivedEventArgs);
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>((source, eventHandler) => source.Test += eventHandler)
                    .IsForwardedToPipeline()
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
