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

        private TestEventsContext1 m_TestEventsContext1;
        private TestEventsContext2 m_TestEventsContext2;

        private EventsScope m_EventsScope;

        [SetUp]
        public void SetUp()
        {
            var appServiceProvider = new ServiceCollection().BuildServiceProvider();
            m_TestEventsContext1 = new TestEventsContext1();
            m_TestEventsContext2 = new TestEventsContext2();

            m_EventsScope = new EventsScope(
                new EventsContext[] {m_TestEventsContext1, m_TestEventsContext2},
                appServiceProvider
            );
        }

        [Test]
        public void ProcessQueuedEventsAsync_ShouldProcessOnlyTestEventsContext1Events()
        {
            object eventsContext1Sender = null;
            TestEventArgs eventsContext1EventArgs = null;
            m_TestEventsContext1.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    eventsContext1Sender = sender;
                    eventsContext1EventArgs = args;
                };
            });

            object eventsContext2Sender = null;
            TestEventArgs eventsContext2EventArgs = null;
            m_TestEventsContext2.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    eventsContext2Sender = sender;
                    eventsContext2EventArgs = args;
                };
            });

            TestUtils.AttachAndRaiseEvent(m_TestEventsContext1, m_EventsScope);
            TestUtils.AttachAndRaiseEvent(m_TestEventsContext2, m_EventsScope);

            m_TestEventsContext1.ProcessQueuedEventsAsync(m_EventsScope, QueueName);

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
                    .IsForwardedToPipeline()
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
                    .IsForwardedToPipeline()
                    .ThenIsQueuedTo(QueueName)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
