using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class ProjectionWithEventSelectorTest
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
        public void ProjectionWithEventSelector_WithSyncEventOnOriginalSourceAndProjection_ShouldWork()
        {
            object receivedSender = null;
            object receivedEventArgs = null;
            _testEventsContext.SubscribeGloballyTo<ProjectedTestEntity>(testEntity =>
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

        [Test]
        public void ProjectionWithEventSelector_WithSyncEventOnOriginalSourceAndAsyncEventOnProjection_ShouldWork()
        {
            object receivedSender = null;
            object receivedEventArgs = null;
            _testEventsContext.SubscribeGloballyTo<ProjectedTestEntity>(testEntity =>
            {
                testEntity.AsyncTest += (sender, args) =>
                {
                    receivedSender = sender;
                    receivedEventArgs = args;

                    return Task.CompletedTask;
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
                    .IsWatched()
                    .ThenIsProjected(
                        source => new ProjectedTestEntity
                        {
                            Id = source.Id
                        },
                        args => new ProjectedEventArgs
                        {
                            Value = args.Value
                        },
                        (source, h) => source.Test += h)
                    .ThenIsPublishedToGlobalSubscriptions();

                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>((source, eventHandler) => source.Test += eventHandler)
                    .IsWatched()
                    .ThenIsProjected(
                        source => new ProjectedTestEntity
                        {
                            Id = source.Id
                        },
                        args => new ProjectedEventArgs
                        {
                            Value = args.Value
                        },
                        (source, h) => source.AsyncTest += h)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
