using System;
using System.Threading.Tasks;
using FluentEvents.Configuration;
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

        private ServiceHandlerConfiguration<SubscribingService, object> _serviceHandlerConfiguration;
        private ServiceHandlerConfiguration<SubscribingService, object> _optionalServiceHandlerConfiguration;

        [SetUp]
        public void SetUp()
        {
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _serviceHandlerConfiguration = new ServiceHandlerConfiguration<SubscribingService, object>(
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionsServiceMock.Object,
                false
            );

            _optionalServiceHandlerConfiguration = new ServiceHandlerConfiguration<SubscribingService, object>(
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionsServiceMock.Object,
                true
            );
        }

        [TearDown]
        public void TearDown()
        {
            _globalSubscriptionsServiceMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
        }

        [Test]
        public void HasGlobalSubscription_ShouldConfigureSubscription([Values] bool isServiceHandlerOptional)
        {
            _globalSubscriptionsServiceMock
                .Setup(x => x.AddGlobalServiceHandlerSubscription<SubscribingService, object>(isServiceHandlerOptional))
                .Verifiable();

            if (isServiceHandlerOptional)
                _optionalServiceHandlerConfiguration.HasGlobalSubscription();
            else
                _serviceHandlerConfiguration.HasGlobalSubscription();
        }
        
        [Test]
        public void HasScopedSubscription_ShouldConfigureSubscription([Values] bool isServiceHandlerOptional)
        {
            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceHandlerSubscription<SubscribingService, object>(isServiceHandlerOptional))
                .Verifiable();

            if (isServiceHandlerOptional)
                _optionalServiceHandlerConfiguration.HasScopedSubscription();
            else
                _serviceHandlerConfiguration.HasScopedSubscription();
        }
        
        private class SubscribingService : IAsyncEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
