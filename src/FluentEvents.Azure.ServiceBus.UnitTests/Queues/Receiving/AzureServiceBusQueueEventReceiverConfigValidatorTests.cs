using FluentEvents.Azure.ServiceBus.Queues.Receiving;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues.Receiving
{
    [TestFixture]
    public class AzureServiceBusQueueEventReceiverConfigValidatorTests
    {
        private AzureServiceBusQueueEventReceiverConfigValidator _azureServiceBusQueueEventReceiverConfigValidator;
        private AzureServiceBusQueueEventReceiverConfig _azureServiceBusQueueEventReceiverConfig;

        [SetUp]
        public void SetUp()
        {
            _azureServiceBusQueueEventReceiverConfigValidator = new AzureServiceBusQueueEventReceiverConfigValidator();
            _azureServiceBusQueueEventReceiverConfig = new AzureServiceBusQueueEventReceiverConfig
            {
                ReceiveConnectionString = Constants.ValidConnectionString,
            };
        }

        [Test]
        public void Validate_WithNullOrEmptyReceiveConnectionString_ShouldFail(
            [Values("", " ", null)] string receiveConnectionString
        )
        {
            _azureServiceBusQueueEventReceiverConfig.ReceiveConnectionString = receiveConnectionString;

            var result = _azureServiceBusQueueEventReceiverConfigValidator.Validate(null, _azureServiceBusQueueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureServiceBusQueueEventReceiverConfig.ReceiveConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidReceiveConnectionString_ShouldFail()
        {
            _azureServiceBusQueueEventReceiverConfig.ReceiveConnectionString = Constants.InvalidConnectionString;

            var result = _azureServiceBusQueueEventReceiverConfigValidator.Validate(null, _azureServiceBusQueueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureServiceBusQueueEventReceiverConfig.ReceiveConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var result = _azureServiceBusQueueEventReceiverConfigValidator.Validate(null, _azureServiceBusQueueEventReceiverConfig);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }
    }
}
