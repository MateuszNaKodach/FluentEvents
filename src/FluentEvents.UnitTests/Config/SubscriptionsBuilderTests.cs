using FluentEvents.Config;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class SubscriptionsBuilderTests
    {
        private Mock<IGlobalSubscriptionCollection> _globalSubscriptionCollection;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsService;

        private SubscriptionsBuilder _subscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionCollection = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            _scopedSubscriptionsService = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _subscriptionsBuilder = new SubscriptionsBuilder(
                _globalSubscriptionCollection.Object,
                _scopedSubscriptionsService.Object
            );
        }

        [Test]
        public void Service_ShouldReturnServiceConfigurator()
        {
            var serviceConfigurator = _subscriptionsBuilder.Service<object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(serviceConfigurator, Is.TypeOf<ServiceConfigurator<object>>());
        }
    }
}
