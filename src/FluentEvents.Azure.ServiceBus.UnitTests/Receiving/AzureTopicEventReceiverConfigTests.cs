using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureTopicEventReceiverConfigTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private AzureTopicEventReceiverConfig m_AzureTopicEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ReceiveConnectionString = ValidConnectionString,
                ManagementConnectionString = ValidConnectionString,
                SubscriptionNameGenerator = () => "",
                TopicPath = "TopicPath"
            };
        }

        [Test]
        public void SubscriptionNameGenerator_WhenNotSet_ShouldReturnGuidByDefault()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig();

            var subscriptionName = m_AzureTopicEventReceiverConfig.SubscriptionNameGenerator();

            Assert.That(subscriptionName, Is.Not.Null);
            Assert.That(Guid.TryParse(subscriptionName, out _), Is.True);
        }

        [Test]
        public void SubscriptionNameGenerator_WhenSetToNull_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
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
                m_AzureTopicEventReceiverConfig.ManagementConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ManagementConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventReceiverConfig.ManagementConnectionString = ValidConnectionString;

            Assert.That(
                m_AzureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ManagementConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_AzureTopicEventReceiverConfig.ReceiveConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ReceiveConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_AzureTopicEventReceiverConfig.ReceiveConnectionString = ValidConnectionString;

            Assert.That(
                m_AzureTopicEventReceiverConfig,
                Has
                    .Property(nameof(AzureTopicEventReceiverConfig.ReceiveConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }

        [Test]
        public void Validate_WhenReceiveConnectionStringIsNull_ShouldThrow()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ManagementConnectionString = ValidConnectionString,
                SubscriptionNameGenerator = m_AzureTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = m_AzureTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig) m_AzureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ReceiveConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenManagementConnectionStringIsNull_ShouldThrow()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ReceiveConnectionString = ValidConnectionString,
                SubscriptionNameGenerator = m_AzureTopicEventReceiverConfig.SubscriptionNameGenerator,
                TopicPath = m_AzureTopicEventReceiverConfig.TopicPath
            };

            Assert.That(() =>
            {
                ((IValidableConfig)m_AzureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<ManagementConnectionStringIsNullException>());
        }

        [Test]
        public void Validate_WhenTopicPathIsNull_ShouldThrow()
        {
            m_AzureTopicEventReceiverConfig = new AzureTopicEventReceiverConfig
            {
                ManagementConnectionString = ValidConnectionString,
                ReceiveConnectionString = ValidConnectionString,
                SubscriptionNameGenerator = m_AzureTopicEventReceiverConfig.SubscriptionNameGenerator
            };

            Assert.That(() =>
            {
                ((IValidableConfig)m_AzureTopicEventReceiverConfig).Validate();
            }, Throws.TypeOf<TopicPathIsNullException>());
        }
    }
}
