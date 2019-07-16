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
                _azureTopicEventSenderConfig.SendConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureTopicEventSenderConfig.SendConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureTopicEventSenderConfig,
                Has
                    .Property(nameof(AzureTopicEventSenderConfig.SendConnectionString))
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
