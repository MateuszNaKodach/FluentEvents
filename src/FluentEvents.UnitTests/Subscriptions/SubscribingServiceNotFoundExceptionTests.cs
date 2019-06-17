using FluentEvents.Subscriptions;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscribingServiceNotFoundExceptionTests
    {
        [Test]
        public void ServiceType_ShouldReturnTypeProvidedInCtor()
        {
            var serviceType = typeof(long);
            var exception = new SubscribingServiceNotFoundException(serviceType);

            Assert.That(
                exception,
                Has.Property(nameof(SubscribingServiceNotFoundException.ServiceType)).EqualTo(serviceType)
            );
        }
    }
}
