using System;
using System.Threading.Tasks;
using AsyncEvent;

namespace FluentEvents.IntegrationTests.Common
{
    public class TestEntity
    {
        public int Id { get; set; }
        public event EventHandler<TestEvent> Test;
        public event AsyncEventHandler<TestEvent> AsyncTest;

        public void RaiseEvent(string value)
        {
            Test?.Invoke(this, new TestEvent {Value = value});
        }

        public async Task RaiseAsyncEvent(string value)
        {
            var asyncTest = AsyncTest;
            if (asyncTest != null)
                await asyncTest.InvokeAsync(this, new TestEvent {Value = value});
        }
    }
}