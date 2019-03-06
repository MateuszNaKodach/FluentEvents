using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureTopicEventSenderConfigTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

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
                m_AzureTopicEventSenderConfig.ConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventSenderConfig.ConnectionString = ValidConnectionString;

            Assert.That(
                m_AzureTopicEventSenderConfig,
                Has
                    .Property(nameof(AzureTopicEventSenderConfig.ConnectionString))
                    .EqualTo(ValidConnectionString)
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
