using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests.Common
{
    public class SubscribingService
        : IEventHandler<TestEvent>,
            IEventHandler<ProjectedTestEvent>,
            IEventHandler<TestEventBase>,
            IEventHandler<TestEvent2>,
            IEventHandler<ITestEvent>
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

        public Task HandleEventAsync(TestEvent domainEvent)
        {
            TestEvents.Add(domainEvent);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(ProjectedTestEvent domainEvent)
        {
            ProjectedTestEvents.Add(domainEvent);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(TestEventBase domainEvent)
        {
            BaseTestEvents.Add(domainEvent);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(TestEvent2 domainEvent)
        {
            TestEvent2s.Add(domainEvent);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(ITestEvent domainEvent)
        {
            ITestEvents.Add(domainEvent);
            return Task.CompletedTask;
        }
    }
}