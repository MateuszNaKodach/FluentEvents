using FluentEvents.Azure.SignalR.Client;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests.Clients
{
    [TestFixture]
    public class ConnectionStringTests
    {
        private string _connectionStringName = "_connectionStringName";
        private const string Endpoint = nameof(Endpoint);
        private const string AccessKey = nameof(AccessKey);

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ToString_ShouldReturnConnectionString()
        {
            var connectionString = new ConnectionString(Endpoint, AccessKey);

            var value = connectionString.ToString();

            Assert.That(value, Is.EqualTo($"{nameof(Endpoint)}={Endpoint};{nameof(AccessKey)}={AccessKey};"));
        }

        [Test]
        [Sequential]
        public void IsValid_WithMissingProperty_ShouldThrow(
            [Values(true, false, true)] bool isEndpointMissing,
            [Values(true, true, false)] bool isAccessKeyMissing
        )
        {
            var connectionString = "";
            if (!isEndpointMissing)
                connectionString += $"{nameof(Endpoint)}={Endpoint};";
            if (!isAccessKeyMissing)
                connectionString += $"{nameof(AccessKey)}={AccessKey};";

            var isValid = ConnectionString.IsValid(connectionString, _connectionStringName, out var errorMessage);

            Assert.That(isValid, Is.False);
            Assert.That(errorMessage, Is.Not.Null);
        }

        [Test]
        [Sequential]
        public void Validate_WithDuplicateProperty_ShouldThrow(
            [Values(true, false, true)] bool isEndpointDuplicated,
            [Values(true, true, false)] bool isAccessKeyDuplicated
        )
        {
            var connectionString = $"{nameof(Endpoint)}={Endpoint};{nameof(AccessKey)}={AccessKey};";
            if (isEndpointDuplicated)
                connectionString += $"{nameof(Endpoint)}={Endpoint};";
            if (isAccessKeyDuplicated)
                connectionString += $"{nameof(AccessKey)}={AccessKey};";

            var isValid = ConnectionString.IsValid(connectionString, _connectionStringName, out var errorMessage);

            Assert.That(isValid, Is.False);
            Assert.That(errorMessage, Is.Not.Null);
        }

        [Test]
        [Sequential]
        public void Validate_WithInvalidChars_ShouldThrow()
        {
            var connectionString = "abc";

            var isValid = ConnectionString.IsValid(connectionString, _connectionStringName, out var errorMessage);

            Assert.That(isValid, Is.False);
            Assert.That(errorMessage, Is.Not.Null);
        }

        [Test]
        [Sequential]
        public void Validate_WithNullValue_ShouldThrow()
        {
            string connectionString = null;

            var isValid = ConnectionString.IsValid(connectionString, _connectionStringName, out var errorMessage);

            Assert.That(isValid, Is.False);
            Assert.That(errorMessage, Is.Not.Null);
        }
    }
}
