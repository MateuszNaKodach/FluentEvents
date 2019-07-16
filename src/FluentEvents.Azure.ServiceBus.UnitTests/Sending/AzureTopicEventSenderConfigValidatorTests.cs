using FluentEvents.Azure.ServiceBus.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureAzureTopicEventSenderConfigValidatorTests
    {
        private AzureTopicEventSenderConfigValidator _topicEventSenderConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _topicEventSenderConfigValidator = new AzureTopicEventSenderConfigValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new AzureTopicEventSenderConfig { SendConnectionString = sendConnectionString };

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventSenderConfig.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new AzureTopicEventSenderConfig
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureTopicEventSenderConfig.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new AzureTopicEventSenderConfig
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }

    }
}
