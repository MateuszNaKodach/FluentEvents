using FluentEvents.Azure.ServiceBus.Sending;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureAzureTopicEventSenderOptionsValidatorTests
    {
        private AzureTopicEventSenderOptionsValidator _topicEventSenderOptionsValidator;

        [SetUp]
        public void SetUp()
        {
            _topicEventSenderOptionsValidator = new AzureTopicEventSenderOptionsValidator();
        }

        [Test]
        public void Validate_WithNullOrEmptySendConnectionString_ShouldFail(
            [Values("", " ", null)] string sendConnectionString
        )
        {
            var options = new AzureTopicEventSenderOptions { SendConnectionString = sendConnectionString };

            var result = _topicEventSenderOptionsValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .EqualTo($"{nameof(AzureTopicEventSenderOptions.SendConnectionString)} is null or empty")
            );
        }

        [Test]
        public void Validate_WitInvalidSendConnectionString_ShouldFail()
        {
            var options = new AzureTopicEventSenderOptions
            {
                SendConnectionString = Constants.InvalidConnectionString
            };

            var result = _topicEventSenderOptionsValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
            Assert.That(result,
                Has
                    .Property(nameof(ValidateOptionsResult.FailureMessage))
                    .SupersetOf($"{nameof(AzureTopicEventSenderOptions.SendConnectionString)} is invalid:")
            );
        }

        [Test]
        public void Validate_WithValidConfig_ShouldSucceed()
        {
            var options = new AzureTopicEventSenderOptions
            {
                SendConnectionString = Constants.ValidConnectionString
            };

            var result = _topicEventSenderOptionsValidator.Validate(null, options);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.FailureMessage)).Null);
        }

    }
}
