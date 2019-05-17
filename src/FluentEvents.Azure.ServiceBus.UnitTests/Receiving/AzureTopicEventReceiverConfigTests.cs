using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureTopicEventReceiverConfigTests
    {
        private AzureTopicEventReceiverConfig _azureTopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
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
            _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig();

            var subscriptionName = _azureTopicEventReceiverConfig.SubscriptionNameGenerator();

            Assert.That(subscriptionName, Is.Not.Null);
            Assert.That(Guid.TryParse(subscriptionName, out _), Is.True);
        }

        [Test]
        public void SubscriptionNameGenerator_WhenSetToNull_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
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
                _azureTopicEventReceiverConfig.ManagementConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureTopicEventReceiverConfig.ManagementConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ManagementConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                _azureTopicEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            _azureTopicEventReceiverConfig.ReceiveConnectionString = Constants.ValidConnectionString;

            Assert.That(
                _azureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ReceiveConnectionString))
                    .EqualTo(Constants.ValidConnectionString)
            );
        }

        [Test]
        public void Validate_WhenReceiveConnectionStringIsNull_ShouldThrow()
        {
            _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = _azureTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig) _azureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ReceiveConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenManagementConnectionStringIsNull_ShouldThrow()
        {
            _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = _azureTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig)_azureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ManagementConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenTopicPathIsNull_ShouldThrow()
        {
            _azureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ManagementConnectionString = Constants.ValidConnectionString,
                ReceiveConnectionString = Constants.ValidConnectionString,
                SubscriptionNameGenerator = _azureTopicEventReceiverConfig.SubscriptionNameGenerator
            };

            Assert.That(() =>
            {
                ((IValidableConfig)_azureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<TopicPathIsNullException>());
        }
    }
}
