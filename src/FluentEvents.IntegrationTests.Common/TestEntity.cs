using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests.Common
{
    public class TestEntity
    {
        public int Id { get; set; }
        public event DomainEventHandler<TestEvent> Test;
        public event AsyncDomainEventHandler<TestEvent> AsyncTest;

        public void RaiseEvent(string value)
        {
            Test?.Invoke(new TestEvent {Value = value});
        }

        public async Task RaiseAsyncEvent(string value)
        {
            var asyncTest = AsyncTest;
            if (asyncTest != null)
                await asyncTest.Invoke(new TestEvent {Value = value});
        }
    }
}