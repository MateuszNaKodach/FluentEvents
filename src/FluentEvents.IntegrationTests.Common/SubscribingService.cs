using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests.Common
{
    public class SubscribingService : IEventHandler<TestEvent>, IEventHandler<ProjectedEvent>
    {
        public IList<TestEvent> Events { get; }
        public IList<ProjectedEvent> ProjectedEvents { get; }

        public SubscribingService()
        {
            Events = new List<TestEvent>();
            ProjectedEvents = new List<ProjectedEvent>();
        }

        public Task HandleEventAsync(TestEvent domainEvent)
        {
            Events.Add(domainEvent);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(ProjectedEvent domainEvent)
        {
            ProjectedEvents.Add(domainEvent);
            return Task.CompletedTask;
        }
    }
}