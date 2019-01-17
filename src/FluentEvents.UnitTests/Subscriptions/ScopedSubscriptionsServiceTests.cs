using System;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ScopedSubscriptionsServiceTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISubscriptionsFactory> m_SubscriptionsFactoryMock;

        private ScopedSubscriptionsService m_ScopedSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SubscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);

            m_ScopedSubscriptionsService = new ScopedSubscriptionsService(m_SubscriptionsFactoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_SubscriptionsFactoryMock.Verify();
            m_ServiceProviderMock.Verify();
        }

        [Test]
        public void ConfigureScopedServiceSubscription_ShouldAddCreationTask()
        {
            m_ScopedSubscriptionsService.ConfigureScopedServiceSubscription<object, object>((service, source) => { });
        }

        [Test]
        public void SubscribeServices_ShouldCreateSubscriptionsForConfiguredServices()
        {
            var subscription = new Subscription(typeof(object));

            m_SubscriptionsFactoryMock
                .Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()))
                .Callback<Action<object>>(x => x.Invoke(new object()))
                .Returns(subscription)
                .Verifiable();

            var service1 = SetUpServiceProviderService(new Service1());
            var service2 = SetUpServiceProviderService(new Service2());

            Service1 subscribedService1 = null;
            Service2 subscribedService2 = null;

            m_ScopedSubscriptionsService.ConfigureScopedServiceSubscription<Service1, object>((service, source) =>
            {
                subscribedService1 = service;
            });

            m_ScopedSubscriptionsService.ConfigureScopedServiceSubscription<Service2, object>((service, source) =>
            {
                subscribedService2 = service;
            });

            var subscriptions = m_ScopedSubscriptionsService.SubscribeServices(m_ServiceProviderMock.Object);

            Assert.That(subscriptions, Has.Exactly(2).Items);
            Assert.That(subscribedService1, Is.EqualTo(service1));
            Assert.That(subscribedService2, Is.EqualTo(service2));
        }

        private T SetUpServiceProviderService<T>(T service)
        {
            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(T)))
                .Returns(service)
                .Verifiable();

            return service;
        }

        private class Service1 { }

        private class Service2 { }
    }
}
