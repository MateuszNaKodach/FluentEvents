using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureTopicEventSenderConfigTests
    {
        private AzureTopicEventSenderConfig m_AzureTopicEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            m_AzureTopicEventSenderConfig = new AzureTopicEventSenderConfig();
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AzureTopicEventSenderConfig.ConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventSenderConfig.ConnectionString = Constants.ValidConnectionString;

            Assert.That(
                m_AzureTopicEventSenderConfig,
                Has
                    .Property(nameof(AzureTopicEventSenderConfig.ConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }

        [Test]
        public void Validate_WhenConnectionStringIsNull_ShouldThrow()
        {
            Assert.That(() =>
            {
                ((IValidableConfig) m_AzureTopicEventSenderConfig).Validate(); 
            }, Throws.TypeOf<ConnectionStringIsNullException>());
        }
    }
}
