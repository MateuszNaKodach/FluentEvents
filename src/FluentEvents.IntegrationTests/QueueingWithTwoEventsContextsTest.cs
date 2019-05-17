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

        private TestEventsContext1 _testEventsContext1;
        private TestEventsContext2 _testEventsContext2;

        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            var appServiceProvider = new ServiceCollection().BuildServiceProvider();
            _testEventsContext1 = new TestEventsContext1();
            _testEventsContext2 = new TestEventsContext2();

            _eventsScope = new EventsScope(
                new EventsContext[] {_testEventsContext1, _testEventsContext2},
                appServiceProvider
            );
        }

        [Test]
        public void ProcessQueuedEventsAsync_ShouldProcessOnlyTestEventsContext1Events()
        {
            object eventsContext1Sender = null;
            TestEventArgs eventsContext1EventArgs = null;
            _testEventsContext1.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    eventsContext1Sender = sender;
                    eventsContext1EventArgs = args;
                };
            });

            object eventsContext2Sender = null;
            TestEventArgs eventsContext2EventArgs = null;
            _testEventsContext2.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    eventsContext2Sender = sender;
                    eventsContext2EventArgs = args;
                };
            });

            TestUtils.AttachAndRaiseEvent(_testEventsContext1, _eventsScope);
            TestUtils.AttachAndRaiseEvent(_testEventsContext2, _eventsScope);

            _testEventsContext1.ProcessQueuedEventsAsync(_eventsScope, QueueName);

            TestUtils.AssertThatEventIsPublishedProperly(eventsContext1Sender, eventsContext1EventArgs);
            Assert.That(eventsContext2Sender, Is.Null);
            Assert.That(eventsContext2EventArgs, Is.Null);
        }

        private class TestEventsContext1 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsWatched()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }

        private class TestEventsContext2 : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsWatched()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
