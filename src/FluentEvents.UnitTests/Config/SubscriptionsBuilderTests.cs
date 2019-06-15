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
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;
        private Mock<IEventSelectionService> _eventSelectionServiceMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;

        private SubscriptionsBuilder _subscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _eventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);

            _subscriptionsBuilder = new SubscriptionsBuilder(
                _globalSubscriptionsServiceMock.Object,
                _scopedSubscriptionsServiceMock.Object,
                _sourceModelsServiceMock.Object,
                _eventSelectionServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsServiceMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
            _eventSelectionServiceMock.Verify();
            _sourceModelsServiceMock.Verify();
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
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(object)))
                .Returns(new SourceModel(typeof(object)))
                .Verifiable();

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
