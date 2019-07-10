using FluentEvents.Azure.ServiceBus.Topics.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Sending
{
    [TestFixture]
    public class TopicEventSenderConfigValidatorTests
    {
        private TopicEventSenderConfigValidator _topicEventSenderConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _topicEventSenderConfigValidator = new TopicEventSenderConfigValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new TopicEventSenderConfig {SendConnectionString = sendConnectionString};

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(TopicEventSenderConfig.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new TopicEventSenderConfig
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(TopicEventSenderConfig.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new TopicEventSenderConfig
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _topicEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
