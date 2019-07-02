using FluentEvents.Azure.ServiceBus.Common;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests
{
    [TestFixture]
    public class ConnectionStringValidatorTests
    {
        [Test]
        public void ValidateOrThrow_WithValidConnectionString_ShouldReturnConnectionString()
        {
            var connectionString = ConnectionStringValidator.ValidateOrThrow(Constants.ValidConnectionString);

            Assert.That(connectionString, Is.EqualTo(Constants.ValidConnectionString));
        }

        [Test]
        public void ValidateOrThrow_WithInvalidConnectionString_ShouldReturnConnectionString(
            [Values(Constants.InvalidConnectionString, "", " ", null)] string connectionString
        )
        {
           Assert.That(() =>
           {
               ConnectionStringValidator.ValidateOrThrow(connectionString);
           }, Throws.TypeOf<ConnectionStringIsInvalidException>());
        }
    }
}
