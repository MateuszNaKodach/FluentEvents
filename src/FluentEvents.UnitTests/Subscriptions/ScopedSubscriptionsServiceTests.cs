using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ScopedSubscriptionsServiceTests
    {
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;

        private ScopedSubscriptionsService _scopedSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);

            _scopedSubscriptionsService = new ScopedSubscriptionsService();
        }

        [TearDown]
        public void TearDown()
        {
            _scopedAppServiceProviderMock.Verify();
        }

        [Test]
        public void ConfigureScopedServiceHandlerSubscription_ShouldAddCreationTask()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<Service1, object>(false);
        }

        [Test]
        public void SubscribeServices_ShouldCreateSubscriptionsForConfiguredServices()
        {
            SetUpServiceProviderService(new Service1());
            SetUpServiceProviderService(new Service2());

            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<Service1, object>(false);

            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<Service2, object>(false);

            var subscriptions = _scopedSubscriptionsService.SubscribeServices(_scopedAppServiceProviderMock.Object);

            Assert.That(subscriptions, Has.Exactly(2).Items);
        }

        private void SetUpServiceProviderService<T>(T service)
        {
            _scopedAppServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<T>)))
                .Returns(new[] {service})
                .Verifiable();
        }

        private class Service1 : IEventHandler<object> {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }

        private class Service2 : IEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
