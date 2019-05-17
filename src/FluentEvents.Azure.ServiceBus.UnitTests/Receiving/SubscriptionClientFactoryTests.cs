using FluentEvents.Azure.ServiceBus.Receiving;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class SubscriptionClientFactoryTests
    {
        private const string SubscriptionName = nameof(SubscriptionName);

        private SubscriptionClientFactory _subscriptionClientFactory;

        [SetUp]
        public void SetUp()
        {
            _subscriptionClientFactory = new SubscriptionClientFactory();
        }

        [Test]
        public void GetNew_ShouldReturnSubscriptionClient()
        {
            var subscriptionClient = _subscriptionClientFactory.GetNew(Constants.ValidConnectionString, SubscriptionName);

            Assert.That(subscriptionClient, Is.TypeOf<SubscriptionClient>());
            Assert.That(
                subscriptionClient,
                Has.Property(nameof(SubscriptionClient.SubscriptionName)).EqualTo(SubscriptionName)
            );
        }
    }
}
