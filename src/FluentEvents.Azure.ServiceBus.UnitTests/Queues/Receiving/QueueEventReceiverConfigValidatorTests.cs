using FluentEvents.Azure.ServiceBus.Queues.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Receiving
{
    [TestFixture]
    public class QueueEventReceiverConfigValidatorTests
    {
        private QueueEventReceiverConfigValidator _queueEventReceiverConfigValidator;
        private QueueEventReceiverConfig _queueEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _queueEventReceiverConfigValidator = new QueueEventReceiverConfigValidator();
            _queueEventReceiverConfig = new QueueEventReceiverConfig
            {
                ReceiveConnectionString = Constants.ValidConnectionString,
            };
        }

        [Test]
        public void Validate_WithNullOrEmptyReceiveConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _queueEventReceiverConfig.ReceiveConnectionString = receiveConnectionString;

            var result = _queueEventReceiverConfigValidator.Validate(null, _queueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(QueueEventReceiverConfig.ReceiveConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidReceiveConnectionString_ShouldFail()
        {
            _queueEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;

            var result = _queueEventReceiverConfigValidator.Validate(null, _queueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(QueueEventReceiverConfig.ReceiveConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var result = _queueEventReceiverConfigValidator.Validate(null, _queueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
