using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests
{
    public class SubscribingService
    {
        public TestEventArgs EventArgs { get; private set; }
        public ProjectedEventArgs ProjectedEventArgs { get; private set; }

        public void Subscribe(TestEntity testEntity)
        {
            testEntity.Test += TestEntityOnTest;
        }

        public void Subscribe(ProjectedTestEntity testEntity)
        {
            testEntity.Test += TestEntityOnTest;
        }

        public void AsyncSubscribe(TestEntity entity)
        {
            entity.AsyncTest += EntityOnAsyncTest;
        }

        public void AsyncSubscribe(ProjectedTestEntity entity)
        {
            entity.AsyncTest += EntityOnAsyncTest;
        }

        private void TestEntityOnTest(object sender, ProjectedEventArgs e)
        {
            ProjectedEventArgs = e;
        }

        private Task EntityOnAsyncTest(object sender, ProjectedEventArgs e)
        {
            ProjectedEventArgs = e;
            return Task.CompletedTask;
        }

        private void TestEntityOnTest(object sender, TestEventArgs e)
        {
            EventArgs = e;
        }

        private Task EntityOnAsyncTest(object sender, TestEventArgs e)
        {
            EventArgs = e;
            return Task.CompletedTask;
        }
    }
}