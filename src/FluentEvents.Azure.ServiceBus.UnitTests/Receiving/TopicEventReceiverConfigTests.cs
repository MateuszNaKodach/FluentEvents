using FluentEvents.Azure.ServiceBus.Receiving;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class TopicEventReceiverConfigTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private AzureTopicEventReceiverConfig m_AzureTopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig();
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AzureTopicEventReceiverConfig.ManagementConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventReceiverConfig.ManagementConnectionString = ValidConnectionString;

            Assert.That(
                m_AzureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ManagementConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AzureTopicEventReceiverConfig.ReceiveConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventReceiverConfig.ReceiveConnectionString = ValidConnectionString;

            Assert.That(
                m_AzureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ReceiveConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }
    }
}
