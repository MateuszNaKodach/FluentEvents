using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class AzureSignalRServiceConfigValidatorTests
    {
        private const string ValidConnectionString = "Endpoint=123;AccessKey=123;";

        private AzureSignalRServiceConfigValidator _azureSignalRServiceConfigValidator;

        [SetUp]
        public void SetUp()
        {
            _azureSignalRServiceConfigValidator = new AzureSignalRServiceConfigValidator();
        }

        [Test]
        public void Validate_WithValidConnectionString_ShouldSucceed()
        {
            var config = new AzureSignalRServiceConfig {ConnectionString = ValidConnectionString};

            var result = _azureSignalRServiceConfigValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
        }

        [Test]
        public void Validate_WithInvalidConnectionString_ShouldFail()
        {
            var config = new AzureSignalRServiceConfig {ConnectionString = "abc"};

            var result = _azureSignalRServiceConfigValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
        }

        [Test]
        public void Validate_WithNullConnectionString_ShouldFail()
        {
            var config = new AzureSignalRServiceConfig();

            var result = _azureSignalRServiceConfigValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
        }
    }
}
