using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Model;
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
        private Mock<ISourceModelsService> _sourceModelsService;

        private SubscriptionsBuilder _subscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionCollection = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            _scopedSubscriptionsService = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _sourceModelsService = new Mock<ISourceModelsService>(MockBehavior.Strict);

            _subscriptionsBuilder = new SubscriptionsBuilder(
                _globalSubscriptionCollection.Object,
                _scopedSubscriptionsService.Object,
                _sourceModelsService.Object
            );
        }

        [Test]
        public void Service_ShouldReturnServiceConfigurator()
        {
            var serviceConfigurator = _subscriptionsBuilder.Service<object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(serviceConfigurator, Is.TypeOf<ServiceConfigurator<object>>());
        }

        [Test]
        public void ServiceHandler_ShouldReturnServiceHandlerConfigurator()
        {
            var serviceConfigurator = _subscriptionsBuilder.ServiceHandler<SubscribingService, object, object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(
                serviceConfigurator,
                Is.TypeOf<ServiceHandlerConfigurator<SubscribingService, object, object>>()
            );
        }

        private class SubscribingService : IEventHandler<object, object>
        {
            public Task HandleEventAsync(object source, object args)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
