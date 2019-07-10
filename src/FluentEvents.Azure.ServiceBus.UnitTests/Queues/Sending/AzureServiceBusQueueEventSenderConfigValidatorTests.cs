using FluentEvents.Azure.ServiceBus.Queues.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Sending
{
    [TestFixture]
    public class AzureServiceBusQueueEventSenderConfigValidatorTests
    {
        private AzureServiceBusQueueEventSenderConfigValidator _azureServiceBusQueueEventSenderConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusQueueEventSenderConfigValidator = new AzureServiceBusQueueEventSenderConfigValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new AzureServiceBusQueueEventSenderConfig {SendConnectionString = sendConnectionString};

            var result = _azureServiceBusQueueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusQueueEventSenderConfig.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new AzureServiceBusQueueEventSenderConfig
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _azureServiceBusQueueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureServiceBusQueueEventSenderConfig.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new AzureServiceBusQueueEventSenderConfig
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _azureServiceBusQueueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
