using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class QueueAndProjectAndPublishLocallyTest : BaseTest<QueueAndProjectAndPublishLocallyTest.TestEventsContext>
    {
        [Test]
        public async Task EventShouldBeQueuedAndProjectedAndPublishedOnCommit()
        {
            ProjectedEventArgs projectedEventArgs = null;
            Context.MakeGlobalSubscriptionsTo<ProjectedTestEntity>(testEntity => testEntity.Test += (sender, args) => { projectedEventArgs = args; });

            Entity.RaiseEvent(TestValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(projectedEventArgs, Is.Not.Null);
            Assert.That(projectedEventArgs, Has.Property(nameof(ProjectedEventArgs.Value)).EqualTo(TestValue));
        }

        [Test]
        public async Task AsyncEventShouldBeQueuedAndProjectedAndPublishedOnCommit()
        {
            ProjectedEventArgs projectedEventArgs = null;
            Context.MakeGlobalSubscriptionsTo<ProjectedTestEntity>(testEntity => testEntity.AsyncTest += (sender, args) =>
            {
                projectedEventArgs = args;
                return Task.CompletedTask;
            });

            await Entity.RaiseAsyncEvent(TestValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(projectedEventArgs, Is.Not.Null);
            Assert.That(projectedEventArgs, Has.Property(nameof(ProjectedEventArgs.Value)).EqualTo(TestValue));
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsForwardedToPipeline()
                    .ThenIsEnqueuedToDefaultQueue()
                    .ThenIsProjected(x => new ProjectedTestEntity
                    {
                        Id = x.Id
                    }, x => new ProjectedEventArgs
                    {
                        Value = x.Value
                    })
                    .ThenIsPublishedToGlobalSubscriptions();

                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.AsyncTest))
                    .IsForwardedToPipeline()
                    .ThenIsEnqueuedToDefaultQueue()
                    .ThenIsProjected(x => new ProjectedTestEntity
                    {
                        Id = x.Id
                    }, x => new ProjectedEventArgs
                    {
                        Value = x.Value
                    })
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
