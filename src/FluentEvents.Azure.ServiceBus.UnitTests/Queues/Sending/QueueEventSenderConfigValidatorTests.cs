using FluentEvents.Azure.ServiceBus.Queues.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Sending
{
    [TestFixture]
    public class QueueEventSenderConfigValidatorTests
    {
        private QueueEventSenderConfigValidator _queueEventSenderConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _queueEventSenderConfigValidator = new QueueEventSenderConfigValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new QueueEventSenderConfig {SendConnectionString = sendConnectionString};

            var result = _queueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(QueueEventSenderConfig.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new QueueEventSenderConfig
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _queueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(QueueEventSenderConfig.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new QueueEventSenderConfig
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _queueEventSenderConfigValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
