using System;
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
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<ISubscriptionsFactory> _subscriptionsFactoryMock;

        private ScopedSubscriptionsService _scopedSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            _subscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);

            _scopedSubscriptionsService = new ScopedSubscriptionsService(_subscriptionsFactoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionsFactoryMock.Verify();
            _appServiceProviderMock.Verify();
        }

        [Test]
        public void ConfigureScopedServiceSubscription_ShouldAddCreationTask()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceSubscription<object, object>((service, source) => { });
        }

        [Test]
        public void ConfigureScopedServiceHandlerSubscription_ShouldAddCreationTask()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<Service1, object, object>("");
        }

        [Test]
        public void SubscribeServices_ShouldCreateSubscriptionsForConfiguredServices()
        {
            var subscription = new Subscription(typeof(object));

            _subscriptionsFactoryMock
                .Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()))
                .Callback<Action<object>>(x => x.Invoke(new object()))
                .Returns(subscription)
                .Verifiable();

            var service1 = SetUpServiceProviderService(new Service1());
            var service2 = SetUpServiceProviderService(new Service2());

            Service1 subscribedService1 = null;
            Service2 subscribedService2 = null;

            _scopedSubscriptionsService.ConfigureScopedServiceSubscription<Service1, object>((service, source) =>
            {
                subscribedService1 = service;
            });

            _scopedSubscriptionsService.ConfigureScopedServiceSubscription<Service2, object>((service, source) =>
            {
                subscribedService2 = service;
            });

            var subscriptions = _scopedSubscriptionsService.SubscribeServices(_appServiceProviderMock.Object);

            Assert.That(subscriptions, Has.Exactly(2).Items);
            Assert.That(subscribedService1, Is.EqualTo(service1));
            Assert.That(subscribedService2, Is.EqualTo(service2));
        }

        private T SetUpServiceProviderService<T>(T service)
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(T)))
                .Returns(service)
                .Verifiable();

            return service;
        }

        private class Service1 : IEventHandler<object, object> {
            public Task HandleEventAsync(object source, object args)
            {
                throw new NotImplementedException();
            }
        }

        private class Service2 { }
    }
}
