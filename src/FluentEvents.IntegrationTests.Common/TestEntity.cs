using System;
using System.Threading.Tasks;
using AsyncEvent;

namespace FluentEvents.IntegrationTests.Common
{
    public class TestEntity
    {
        public int Id { get; set; }
        public event EventHandler<TestEventArgs> Test;
        public event AsyncEventHandler<TestEventArgs> AsyncTest;

        public void RaiseEvent(string value)
        {
            Test?.Invoke(this, new TestEventArgs {Value = value});
        }

        public async Task RaiseAsyncEvent(string value)
        {
            var asyncTest = AsyncTest;
            if (asyncTest != null)
                await asyncTest.InvokeAsync(this, new TestEventArgs {Value = value});
        }
    }
}