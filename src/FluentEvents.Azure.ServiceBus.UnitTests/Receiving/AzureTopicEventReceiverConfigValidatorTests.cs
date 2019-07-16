using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureAzureTopicEventReceiverConfigValidatorTests
    {
        private AzureTopicEventReceiverConfigValidator _topicEventReceiverConfigValidator;
        private AzureTopicEventReceiverConfig _topicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _topicEventReceiverConfigValidator = new AzureTopicEventReceiverConfigValidator();
            _topicEventReceiverConfig = new AzureTopicEventReceiverConfig
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
                    .EqualTo($"{nameof(AzureTopicEventReceiverConfig.SubscriptionNameGenerator)} is null")
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
                    .EqualTo($"{nameof(AzureTopicEventReceiverConfig.ReceiveConnectionString)} is null or empty")
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
                    .SupersetOf($"{nameof(AzureTopicEventReceiverConfig.ReceiveConnectionString)} is invalid:")
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
                    .EqualTo($"{nameof(AzureTopicEventReceiverConfig.ManagementConnectionString)} is null or empty")
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
                    .SupersetOf($"{nameof(AzureTopicEventReceiverConfig.ManagementConnectionString)} is invalid:")
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
                    .EqualTo($"{nameof(AzureTopicEventReceiverConfig.TopicPath)} is null or empty")
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
