using FluentEvents.Azure.ServiceBus.Common;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Common
{
    [TestFixture]
    public class ConnectionStringValidatorTests
    {
        private string _connectionStringName = "connectionStringName";

        [Test]
        public void IsValid_WithValidConnectionString_ShouldReturnTrueAndNullErrorMessage()
        {
            var result = ConnectionStringValidator.IsValid(
                Constants.ValidConnectionString,
                _connectionStringName, 
                out var errorMessage
            );

            Assert.That(result, Is.True);
            Assert.That(errorMessage, Is.Null);
        }

        [Test]
        public void IsValid_WithNullOrEmptyConnectionString_ShouldReturnFalseAndEmptyErrorMessage(
            [Values("", " ", null)] string connectionString
        )
        {
            var result = ConnectionStringValidator.IsValid(connectionString, _connectionStringName, out var errorMessage);

            Assert.That(result, Is.False);
            Assert.That(errorMessage, Is.EqualTo(_connectionStringName + " is null or empty"));
        }


        [Test]
        public void IsValid_WithInvalidConnectionString_ShouldReturnFalseAndEmptyErrorMessage()
        {
            var result = ConnectionStringValidator.IsValid(
                Constants.InvalidConnectionString,
                _connectionStringName,
                out var errorMessage
            );

            Assert.That(result, Is.False);
            Assert.That(errorMessage, Is.SupersetOf(_connectionStringName + " is invalid: "));
        }
    }
}
