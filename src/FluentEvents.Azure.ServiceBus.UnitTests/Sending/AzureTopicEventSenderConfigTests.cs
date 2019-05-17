using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureTopicEventSenderConfigTests
    {
        private AzureTopicEventSenderConfig _azureTopicEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            _azureTopicEventSenderConfig = new AzureTopicEventSenderConfig();
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureTopicEventSenderConfig.ConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureTopicEventSenderConfig.ConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureTopicEventSenderConfig,
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
                ((IValidableConfig) _azureTopicEventSenderConfig).Validate(); 
            }, Throws.TypeOf<ConnectionStringIsNullException>());
        }
    }
}
