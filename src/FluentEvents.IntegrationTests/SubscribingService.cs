using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests
{
    public class SubscribingService
    {
        public TestEventArgs EventArgs { get; private set; }

        public void Subscribe(TestEntity testEntity)
        {
            testEntity.Test += TestEntityOnTest;
        }

        private void TestEntityOnTest(object sender, TestEventArgs e)
        {
            EventArgs = e;
        }

        public void AsyncSubscribe(TestEntity entity)
        {
            entity.AsyncTest += EntityOnAsyncTest;
        }

        private Task EntityOnAsyncTest(object sender, TestEventArgs e)
        {
            EventArgs = e;
            return Task.CompletedTask;
        }
    }
}