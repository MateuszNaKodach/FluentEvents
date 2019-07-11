using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.IntegrationTests
{
    [TestFixture]
    public class SendToAzureSignalRUsersTest 
        : SendToAzureSignalRTestBase<SendToAzureSignalRUsersTest.TestEventsContext>
    {
        [Test]
        public async Task Test()
        {
            var semaphoreSlim = new SemaphoreSlim(3);
            var task1 = CheckEventPublishing(HubConnection1, semaphoreSlim, true);
            var task2 = CheckEventPublishing(HubConnection2, semaphoreSlim, true);
            var task3 = CheckEventPublishing(HubConnection3, semaphoreSlim, false);
            var allTasks = Task.WhenAll(task1, task2, task3);

            TestUtils.AttachAndRaiseEvent(EventsContext, Scope);
            semaphoreSlim.Release(3);

            await allTasks;

            Assert.That(ReceivedEventsCount, Is.EqualTo(2));
        }

        public class TestEventsContext : EventsContext
        {
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEvent>(nameof(TestEntity.Test))
                    .IsPiped()
                    .ThenIsSentToAzureSignalRUsers((sender, args) => new[] { UserId1, UserId2 }, HubName, HubMethodName);
            }
        }
    }
}
