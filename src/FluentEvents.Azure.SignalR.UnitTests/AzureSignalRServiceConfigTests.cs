using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class AzureSignalRServiceConfigTests
    {
        private const string ValidConnectionString = "Endpoint=123;AccessKey=123;";

        [Test]
        public void ConnectionString_WithValidConnectionString_ShouldSet()
        {
            var config = new AzureSignalRServiceConfig {ConnectionString = ValidConnectionString};

            Assert.That(config.ConnectionString, Is.EqualTo(ValidConnectionString));
        }

        [Test]
        public void ConnectionString_WithInvalidConnectionString_ShouldThrow()
        {
            var config = new AzureSignalRServiceConfig();

            Assert.That(() =>
            {
                config.ConnectionString = "";
            }, Throws.TypeOf<ConnectionStringHasMissingPropertiesException>());
        }

        [Test]
        public void ConnectionString_WithNullConnectionString_ShouldThrow()
        {
            var config = new AzureSignalRServiceConfig();

            Assert.That(() =>
            {
                config.ConnectionString = null;
            }, Throws.TypeOf<ConnectionStringIsNullException>());
        }


        [Test]
        public void Validate_WithNullConnectionString_ShouldThrow()
        {
            var config = new AzureSignalRServiceConfig();

            Assert.That(() =>
            {
                ((IValidableConfig) config).Validate();
            }, Throws.TypeOf<ConnectionStringIsNullException>());
        }
    }
}
