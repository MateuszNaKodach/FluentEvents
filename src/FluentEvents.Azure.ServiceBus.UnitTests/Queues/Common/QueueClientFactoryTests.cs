using FluentEvents.Azure.ServiceBus.Queues.Common;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Common
{
    [TestFixture]
    public class QueueClientFactoryTests
    {
        private QueueClientFactory _queueClientFactory;

        [SetUp]
        public void SetUp()
        {
            _queueClientFactory = new QueueClientFactory();
        }

        [Test]
        public void GetNew_ShouldReturnNewQueueClient()
        {
            var queueClient = _queueClientFactory.GetNew(Constants.ValidConnectionString);

            Assert.That(queueClient, Is.TypeOf<QueueClient>());
        }
    }
}
