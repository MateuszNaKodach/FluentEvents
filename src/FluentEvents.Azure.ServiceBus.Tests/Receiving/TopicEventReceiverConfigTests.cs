using FluentEvents.Azure.ServiceBus.Receiving;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.Tests.Receiving
{
    [TestFixture]
    public class TopicEventReceiverConfigTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private TopicEventReceiverConfig m_TopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            m_TopicEventReceiverConfig = new TopicEventReceiverConfig();
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_TopicEventReceiverConfig.ManagementConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_TopicEventReceiverConfig.ManagementConnectionString = ValidConnectionString;

            Assert.That(
                m_TopicEventReceiverConfig,
                Has
                    .Property(nameof(TopicEventReceiverConfig.ManagementConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_TopicEventReceiverConfig.ReceiveConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_TopicEventReceiverConfig.ReceiveConnectionString = ValidConnectionString;

            Assert.That(
                m_TopicEventReceiverConfig,
                Has
                    .Property(nameof(TopicEventReceiverConfig.ReceiveConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }
    }
}
