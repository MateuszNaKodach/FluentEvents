using FluentEvents.Azure.ServiceBus.Sending;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class TopicClientFactoryTests
    {
        private TopicClientFactory _topicClientFactory;

        [SetUp]
        public void SetUp()
        {
            _topicClientFactory = new TopicClientFactory();
        }

        [Test]
        public void GetNew_ShouldReturnTopicClient()
        {
            var topicClient = _topicClientFactory.GetNew(Constants.ValidConnectionString);

            Assert.That(topicClient, Is.TypeOf<TopicClient>());
        }
    }
}
