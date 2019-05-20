using System;
using AsyncEvent;

namespace FluentEvents.IntegrationTests.Common
{
    public class ProjectedTestEntity
    {
        public int Id { get; set; }
        public event EventHandler<ProjectedEventArgs> Test;
        public event AsyncEventHandler<ProjectedEventArgs> AsyncTest;
    }
}
