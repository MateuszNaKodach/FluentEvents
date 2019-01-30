using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class QueueAndPublishLocallyTest : BaseTest<QueueAndPublishLocallyTest.TestEventsContext>
    {
        [Test]
        public async Task EventShouldBeQueuedAndPublishedOnCommit()
        {
            TestEventArgs testEventArgs = null;
            Context.MakeGlobalSubscriptionTo<TestEntity>(testEntity => testEntity.Test += (sender, args) => { testEventArgs = args; });

            Entity.RaiseEvent(TestValue);
            
            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(testEventArgs, Is.Not.Null);
            Assert.That(testEventArgs, Has.Property(nameof(TestEventArgs.Value)).EqualTo(TestValue));
        }

        [Test]
        public async Task AsyncEventShouldBeQueuedAndPublishedOnCommit()
        {
            TestEventArgs testEventArgs = null;
            Context.MakeGlobalSubscriptionTo<TestEntity>(
                testEntity => testEntity.AsyncTest += (sender, args) =>
                {
                    testEventArgs = args;
                    return Task.CompletedTask;
                }
            );

            await Entity.RaiseAsyncEvent(TestValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(testEventArgs, Is.Not.Null);
            Assert.That(testEventArgs, Has.Property(nameof(TestEventArgs.Value)).EqualTo(TestValue));
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsQueuedToDefaultQueue()
                    .ThenIsPublishedToGlobalSubscriptions();

                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.AsyncTest))
                    .IsQueuedToDefaultQueue()
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
