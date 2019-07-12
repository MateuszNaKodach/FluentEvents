using System;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class ServiceHandlerConfiguratorTests
    {
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;

        private ServiceHandlerConfigurator<SubscribingService, object> _serviceHandlerConfigurator;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _serviceHandlerConfigurator = new ServiceHandlerConfigurator<SubscribingService, object>(
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionsServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsServiceMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
        }

        [Test]
        public void HasGlobalSubscription_ShouldConfigureSubscription()
        {
            _globalSubscriptionsServiceMock
                .Setup(x => x.AddGlobalServiceHandlerSubscription<SubscribingService, object>()
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasGlobalSubscription();
        }
        
        [Test]
        public void HasScopedSubscription_ShouldConfigureSubscription()
        {
            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceHandlerSubscription<SubscribingService, object>()
                )
                .Verifiable();

            _serviceHandlerConfigurator.HasScopedSubscription();
        }
        
        private class SubscribingService : IEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
