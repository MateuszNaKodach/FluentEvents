using System;
using FluentEvents.Azure.ServiceBus.Topics.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Receiving
{
    [TestFixture]
    public class TopicEventReceiverConfigValidatorTests
    {
        private TopicEventReceiverConfigValidator _topicEventReceiverConfigValidator;
        private TopicEventReceiverConfig _topicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _topicEventReceiverConfigValidator = new TopicEventReceiverConfigValidator();
            _topicEventReceiverConfig = new TopicEventReceiverConfig
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.MaxValue,
                TopicPath = "TopicPath"
            };
        }

        [Test]
        public void Validate_WithNullSubscriptionNameGenerator_ShouldFail()
        {
            _topicEventReceiverConfig.SubscriptionNameGenerator = null;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(TopicEventReceiverConfig.SubscriptionNameGenerator)} is null")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyReceiveConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _topicEventReceiverConfig.ReceiveConnectionString = receiveConnectionString;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(TopicEventReceiverConfig.ReceiveConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidReceiveConnectionString_ShouldFail()
        {
            _topicEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(TopicEventReceiverConfig.ReceiveConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyManagementConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _topicEventReceiverConfig.ManagementConnectionString = receiveConnectionString;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(TopicEventReceiverConfig.ManagementConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidManagementConnectionString_ShouldFail()
        {
            _topicEventReceiverConfig.ManagementConnectionString = Constants.InvalidConnectionString;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(TopicEventReceiverConfig.ManagementConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyTopicPath_ShouldFail(
            [Values("", " ", null)] string topicPath
        )
        {
            _topicEventReceiverConfig.TopicPath = topicPath;

            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(TopicEventReceiverConfig.TopicPath)} is null or empty")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var result = _topicEventReceiverConfigValidator.Validate(null, _topicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
