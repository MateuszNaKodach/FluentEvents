using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests
{
    [TestFixture]
    public class ConnectionStringValidatorTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        [Test]
        public void ValidateOrThrow_WithValidConnectionString_ShouldReturnConnectionString()
        {
            var connectionString = ConnectionStringValidator.ValidateOrThrow(ValidConnectionString);

            Assert.That(connectionString, Is.EqualTo(ValidConnectionString));
        }

        [Test]
        public void ValidateOrThrow_WithInvalidConnectionString_ShouldReturnConnectionString(
            [Values(InvalidConnectionString, "", " ", null)] string connectionString
        )
        {
           Assert.That(() =>
           {
               ConnectionStringValidator.ValidateOrThrow(connectionString);
           }, Throws.TypeOf<InvalidConnectionStringException>());
        }
    }
}
