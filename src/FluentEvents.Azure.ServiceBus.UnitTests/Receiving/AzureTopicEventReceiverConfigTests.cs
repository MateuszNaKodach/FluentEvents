using System;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Topics.Receiving;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureTopicEventReceiverConfigTests
    {
        private AzureServiceBusTopicEventReceiverConfig _azureServiceBusTopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
            {
                ReceiveConnectionString = Constants.ValidConnectionString,
                ManagementConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = () => "",
                TopicPath = "TopicPath"
            };
        }

        [Test]
        public void SubscriptionNameGenerator_WhenNotSet_ShouldReturnGuidByDefault()
        {
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig();

            var subscriptionName = _azureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator();

            Assert.That(subscriptionName, Is.Not.Null);
            Assert.That(Guid.TryParse(subscriptionName, out _), Is.True);
        }

        [Test]
        public void SubscriptionNameGenerator_WhenSetToNull_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
                {
                    SubscriptionNameGenerator = null
                };
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureServiceBusTopicEventReceiverConfig.ManagementConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<ConnectionStringIsInvalidException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureServiceBusTopicEventReceiverConfig.ManagementConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureServiceBusTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureServiceBusTopicEventReceiverConfig.ManagementConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureServiceBusTopicEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<ConnectionStringIsInvalidException>());
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureServiceBusTopicEventReceiverConfig.ReceiveConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureServiceBusTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureServiceBusTopicEventReceiverConfig.ReceiveConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }

        [Test]
        public void Validate_WhenReceiveConnectionStringIsNull_ShouldThrow()
        {
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = _azureServiceBusTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig) _azureServiceBusTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ReceiveConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenManagementConnectionStringIsNull_ShouldThrow()
        {
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
            {
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = _azureServiceBusTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig)_azureServiceBusTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ManagementConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenTopicPathIsNull_ShouldThrow()
        {
            _azureServiceBusTopicEventReceiverConfig = new AzureServiceBusTopicEventReceiverConfig
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator
            };

            Assert.That(() =>
            {
                ((IValidableConfig)_azureServiceBusTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<TopicPathIsNullException>());
        }
    }
}
