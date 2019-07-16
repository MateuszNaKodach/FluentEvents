using FluentEvents.Azure.SignalR.Client;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class AzureSignalRServiceOptionsValidatorTests
    {
        private const string ValidConnectionString = "Endpoint=123;AccessKey=123;";

        private AzureSignalRServiceOptionsValidator _azureSignalRServiceOptionsValidator;

        [SetUp]
        public void SetUp()
        {
            _azureSignalRServiceOptionsValidator = new AzureSignalRServiceOptionsValidator();
        }

        [Test]
        public void Validate_WithValidConnectionString_ShouldSucceed()
        {
            var config = new AzureSignalRServiceOptions {ConnectionString = ValidConnectionString};

            var result = _azureSignalRServiceOptionsValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(false));
        }

        [Test]
        public void Validate_WithInvalidConnectionString_ShouldFail()
        {
            var config = new AzureSignalRServiceOptions {ConnectionString = "abc"};

            var result = _azureSignalRServiceOptionsValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
        }

        [Test]
        public void Validate_WithNullConnectionString_ShouldFail()
        {
            var config = new AzureSignalRServiceOptions();

            var result = _azureSignalRServiceOptionsValidator.Validate(null, config);

            Assert.That(result, Has.Property(nameof(ValidateOptionsResult.Failed)).EqualTo(true));
        }
    }
}
