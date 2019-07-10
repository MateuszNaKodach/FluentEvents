using System;
using FluentEvents.Azure.ServiceBus.Topics.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Receiving
{
    [TestFixture]
    public class AzureServiceBusTopicEventReceiverConfigValidatorTests
    {
        private AzureServiceBusTopicEventReceiverConfigValidator _azureServiceBusTopicEventReceiverConfigValidator;
        private AzureServiceBusTopicEventReceiverConfig _azureServiceBusTopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusTopicEventReceiverConfigValidator = new AzureServiceBusTopicEventReceiverConfigValidator();
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
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
            _azureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator = null;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator)} is null")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyReceiveConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _azureServiceBusTopicEventReceiverConfig.ReceiveConnectionString = receiveConnectionString;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusTopicEventReceiverConfig.ReceiveConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidReceiveConnectionString_ShouldFail()
        {
            _azureServiceBusTopicEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureServiceBusTopicEventReceiverConfig.ReceiveConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyManagementConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _azureServiceBusTopicEventReceiverConfig.ManagementConnectionString = receiveConnectionString;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusTopicEventReceiverConfig.ManagementConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidManagementConnectionString_ShouldFail()
        {
            _azureServiceBusTopicEventReceiverConfig.ManagementConnectionString = Constants.InvalidConnectionString;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureServiceBusTopicEventReceiverConfig.ManagementConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyTopicPath_ShouldFail(
            [Values("", " ", null)] string topicPath
        )
        {
            _azureServiceBusTopicEventReceiverConfig.TopicPath = topicPath;

            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusTopicEventReceiverConfig.TopicPath)} is null or empty")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var result = _azureServiceBusTopicEventReceiverConfigValidator.Validate(null, _azureServiceBusTopicEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
