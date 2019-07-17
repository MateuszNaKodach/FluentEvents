using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureAzureTopicEventReceiverOptionsValidatorTests
    {
        private AzureTopicEventReceiverOptionsValidator _topicEventReceiverOptionsValidator;
        private AzureTopicEventReceiverOptions _topicEventReceiverOptions;

        [SetUp]
        public void SetUp()
        {
            _topicEventReceiverOptionsValidator = new AzureTopicEventReceiverOptionsValidator();
            _topicEventReceiverOptions = new AzureTopicEventReceiverOptions
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.MaxValue,
                TopicPath = "TopicPath"
            };
        }

        [Test]
        public void Validate_WithNullSubscriptionNameAndSubscriptionNameProvider_ShouldFail(
            [Values("", " ", null)] string subscriptionName
        )
        {
            _topicEventReceiverOptions.SubscriptionName = subscriptionName;
            _topicEventReceiverOptions.SubscriptionNameProvider = null;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventReceiverOptions.SubscriptionName)}" +
                             $" and {nameof(AzureTopicEventReceiverOptions.SubscriptionNameProvider)}" +
                             $" are null or empty," +
                             $" please specify at least one of the parameters"
                    )
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyReceiveConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _topicEventReceiverOptions.ReceiveConnectionString = receiveConnectionString;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventReceiverOptions.ReceiveConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidReceiveConnectionString_ShouldFail()
        {
            _topicEventReceiverOptions.ReceiveConnectionString = Constants.InvalidConnectionString;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureTopicEventReceiverOptions.ReceiveConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyManagementConnectionStringAndSubscriptionCreationEnabled_ShouldFail(
            [Values("", " ", null)] string managementConnectionString
        )
        {
            _topicEventReceiverOptions.IsSubscriptionCreationEnabled = true;
            _topicEventReceiverOptions.ManagementConnectionString = managementConnectionString;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventReceiverOptions.ManagementConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyOrInvalidManagementConnectionStringAndSubscriptionCreationDisabled_ShouldSucceed(
            [Values("", " ", null, Constants.InvalidConnectionString)] string managementConnectionString
        )
        {
            _topicEventReceiverOptions.ManagementConnectionString = managementConnectionString;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }

        [Test]
        public void Validate_WitInvalidManagementConnectionStringAndSubscriptionCreationEnabled_ShouldFail()
        {
            _topicEventReceiverOptions.IsSubscriptionCreationEnabled = true;
            _topicEventReceiverOptions.ManagementConnectionString = Constants.InvalidConnectionString;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureTopicEventReceiverOptions.ManagementConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithNullOrEmptyTopicPath_ShouldFail(
            [Values("", " ", null)] string topicPath
        )
        {
            _topicEventReceiverOptions.TopicPath = topicPath;

            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventReceiverOptions.TopicPath)} is null or empty")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var result = _topicEventReceiverOptionsValidator.Validate(null, _topicEventReceiverOptions);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }

    }
}
