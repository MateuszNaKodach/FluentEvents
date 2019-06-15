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
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsService;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsService;
        private Mock<IEventSelectionService> _eventSelectionService;
        private Mock<ISourceModelsService> _sourceModelsService;

        private SubscriptionsBuilder _subscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsService = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsService = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _eventSelectionService = new Mock<IEventSelectionService>(MockBehavior.Strict);
            _sourceModelsService = new Mock<ISourceModelsService>(MockBehavior.Strict);

            _subscriptionsBuilder = new SubscriptionsBuilder(
                _globalSubscriptionsService.Object,
                _scopedSubscriptionsService.Object,
                _sourceModelsService.Object,
                _eventSelectionService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsService.Verify();
            _scopedSubscriptionsService.Verify();
            _eventSelectionService.Verify();
            _sourceModelsService.Verify();
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
            _sourceModelsService
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
