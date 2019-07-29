using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class SubscriptionsBuilderTests
    {
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;

        private SubscriptionsBuilder _subscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _subscriptionsBuilder = new SubscriptionsBuilder(
                _globalSubscriptionsServiceMock.Object,
                _scopedSubscriptionsServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsServiceMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
        }
        
        [Test]
        public void ServiceHandler_ShouldReturnServiceHandlerConfigurator()
        {
            var serviceConfigurator = _subscriptionsBuilder.ServiceHandler<SubscribingService, object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(
                serviceConfigurator,
                Is.TypeOf<ServiceHandlerConfiguration<SubscribingService, object>>()
            );
        }

        [Test]
        public void OptionalServiceHandler_ShouldReturnServiceHandlerConfigurator()
        {
            var serviceConfigurator = _subscriptionsBuilder.OptionalServiceHandler<SubscribingService, object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(
                serviceConfigurator,
                Is.TypeOf<ServiceHandlerConfiguration<SubscribingService, object>>()
            );
        }

        private class SubscribingService : IAsyncEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
