using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests.Common
{
    public class SubscribingService
        : IAsyncEventHandler<TestEvent>,
            IAsyncEventHandler<ProjectedTestEvent>,
            IAsyncEventHandler<TestEventBase>,
            IAsyncEventHandler<TestEvent2>,
            IAsyncEventHandler<ITestEvent>
    {
        public IList<TestEventBase> BaseTestEvents { get; }
        public IList<TestEvent> TestEvents { get; }
        public IList<TestEvent2> TestEvent2s { get; }
        public IList<ITestEvent> ITestEvents { get; }
        public IList<ProjectedTestEvent> ProjectedTestEvents { get; }

        public SubscribingService()
        {
            BaseTestEvents = new List<TestEventBase>();
            TestEvents = new List<TestEvent>();
            TestEvent2s = new List<TestEvent2>();
            ITestEvents = new List<ITestEvent>();
            ProjectedTestEvents = new List<ProjectedTestEvent>();
        }

        public Task HandleEventAsync(TestEvent e)
        {
            TestEvents.Add(e);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(ProjectedTestEvent e)
        {
            ProjectedTestEvents.Add(e);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(TestEventBase e)
        {
            BaseTestEvents.Add(e);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(TestEvent2 e)
        {
            TestEvent2s.Add(e);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(ITestEvent e)
        {
            ITestEvents.Add(e);
            return Task.CompletedTask;
        }
    }
}