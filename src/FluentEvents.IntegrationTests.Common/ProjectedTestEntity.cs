using System;
using AsyncEvent;

namespace FluentEvents.IntegrationTests.Common
{
    public class ProjectedTestEntity
    {
        public int Id { get; set; }
        public event EventHandler<ProjectedEvent> Test;
        public event AsyncEventHandler<ProjectedEvent> AsyncTest;
    }
}
