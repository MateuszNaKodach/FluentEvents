using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Pipelines.Filters;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    [TestFixture]
    public class QueueAndFilterAndPublishLocallyTest : BaseTest<QueueAndFilterAndPublishLocallyTest.TestEventsContext>
    {
        public const string ValidValue = "ValidValue";
        public const string InvalidValue = "InvalidValue";

        [Test]
        public async Task EventShouldBeQueuedAndPublishedOnCommit([Values(ValidValue, InvalidValue)] string argsValue)
        {
            TestEventArgs testEventArgs = null;
            Context.MakeGlobalSubscriptionTo<TestEntity>(
                testEntity => testEntity.Test += (sender, args) => { testEventArgs = args; });

            Entity.RaiseEvent(argsValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            if (argsValue == ValidValue)
            {
                Assert.That(testEventArgs, Is.Not.Null);
                Assert.That(testEventArgs, Has.Property(nameof(TestEventArgs.Value)).EqualTo(argsValue));
            }
            else
            {
                Assert.That(testEventArgs, Is.Null);
            }
        }

        [Test]
        public async Task AsyncEventShouldBeQueuedAndPublishedOnCommit([Values(ValidValue, InvalidValue)] string argsValue)
        {
            TestEventArgs testEventArgs = null;
            Context.MakeGlobalSubscriptionTo<TestEntity>(
                testEntity => testEntity.AsyncTest += (sender, args) =>
                {
                    testEventArgs = args;
                    return Task.CompletedTask;
                });

            await Entity.RaiseAsyncEvent(argsValue);

            await Context.ProcessQueuedEventsAsync(Scope);

            if (argsValue == ValidValue)
            {
                Assert.That(testEventArgs, Is.Not.Null);
                Assert.That(testEventArgs, Has.Property(nameof(TestEventArgs.Value)).EqualTo(argsValue));
            }
            else
            {
                Assert.That(testEventArgs, Is.Null);
            }
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder.Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsQueuedToDefaultQueue()
                    .ThenIsFiltered((sender, args) => args.Value == ValidValue)
                    .ThenIsPublishedToGlobalSubscriptions();

                pipelinesBuilder.Event<TestEntity, TestEventArgs>(nameof(TestEntity.AsyncTest))
                    .IsQueuedToDefaultQueue()
                    .ThenIsFiltered((sender, args) => args.Value == ValidValue)
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
