using System;
using System.Linq;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class QueueingWithTwoEventsContextsTest
    {
        private const string QueueName = nameof(QueueName);

        private IServiceProvider _appServiceProvider;

        private TestEventsContext1 _testEventsContext1;
        private TestEventsContext2 _testEventsContext2;

        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddSingleton<SubscribingService>();
            _appServiceProvider = services.BuildServiceProvider();

            _testEventsContext1 = new TestEventsContext1();
            _testEventsContext2 = new TestEventsContext2();

            _eventsScope = new EventsScope(
                new EventsContext[] {_testEventsContext1, _testEventsContext2},
                _appServiceProvider
            );
        }

        [Test]
        public void ProcessQueuedEventsAsync_ShouldProcessOnlyTestEventsContext1Events()
        {
            var subscribingService = _appServiceProvider.GetRequiredService<SubscribingService>();

            TestUtils.AttachAndRaiseEvent(_testEventsContext1, _eventsScope);
            TestUtils.AttachAndRaiseEvent(_testEventsContext2, _eventsScope);

            _testEventsContext1.ProcessQueuedEventsAsync(_eventsScope, QueueName);

            TestUtils.AssertThatEventIsPublishedProperly(subscribingService.Events.FirstOrDefault());

            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.Events)).With.One.Items);
        }

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
