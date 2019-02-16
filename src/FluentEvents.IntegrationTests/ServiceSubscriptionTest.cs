using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class ServiceSubscriptionTest : BaseTest<ServiceSubscriptionTest.TestEventsContext>
    {
        [Test]
        public async Task EventShouldBeQueuedAndPublishedOnCommit()
        {
            Entity.RaiseEvent(TestValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(SubscribingService, Has.Property(nameof(SubscribingService.EventArgs)).Not.Null);
        }

        [Test]
        public async Task AsyncEventShouldBeQueuedAndPublishedOnCommit()
        {
            await Entity.RaiseAsyncEvent(TestValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            Assert.That(SubscribingService, Has.Property(nameof(SubscribingService.EventArgs)).Not.Null);
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .Service<SubscribingService>()
                    .HasScopedSubscription<TestEntity>((service, entity) => service.Subscribe(entity))
                    .HasScopedSubscription<TestEntity>((service, entity) => service.AsyncSubscribe(entity));
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .ForwardToPipeline()
                    .ThenEnqueueToDefaultQueue()
                    .ThenPublishToScopedSubscriptions();

                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.AsyncTest))
                    .ForwardToPipeline()
                    .ThenEnqueueToDefaultQueue()
                    .ThenPublishToScopedSubscriptions();
            }
        }
    }
}
