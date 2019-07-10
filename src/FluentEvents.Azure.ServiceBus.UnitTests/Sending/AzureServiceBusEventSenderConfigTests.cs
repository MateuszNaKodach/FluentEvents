using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Topics.Sending;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureServiceBusEventSenderConfigTests
    {
        private AzureServiceBusTopicEventSenderConfig _azureServiceBusTopicEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusTopicEventSenderConfig = new AzureServiceBusTopicEventSenderConfig();
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureServiceBusTopicEventSenderConfig.SendConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<ConnectionStringIsInvalidException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureServiceBusTopicEventSenderConfig.SendConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureServiceBusTopicEventSenderConfig,
                Has
                    .Property(nameof(AzureServiceBusTopicEventSenderConfig.SendConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }
    }
}
