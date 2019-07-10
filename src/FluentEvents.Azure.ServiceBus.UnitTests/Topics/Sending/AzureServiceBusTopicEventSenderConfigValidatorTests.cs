using FluentEvents.Azure.ServiceBus.Topics.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Sending
{
    [TestFixture]
    public class AzureServiceBusTopicEventSenderConfigValidatorTests
    {
        private AzureServiceBusTopicEventSenderConfigValidator _azureServiceBusTopicEventSenderConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusTopicEventSenderConfigValidator = new AzureServiceBusTopicEventSenderConfigValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new AzureServiceBusTopicEventSenderConfig {SendConnectionString = sendConnectionString};

            var result = _azureServiceBusTopicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusTopicEventSenderConfig.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new AzureServiceBusTopicEventSenderConfig
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _azureServiceBusTopicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureServiceBusTopicEventSenderConfig.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new AzureServiceBusTopicEventSenderConfig
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _azureServiceBusTopicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
