using System;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ServiceSubscriptionCreationTaskTests
    {
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<ISubscriptionsFactory> _subscriptionsFactoryMock;
        private Subscription _subscription;

        private ISubscriptionCreationTask _subscriptionCreationTask;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            _subscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);
            _subscription = new Subscription(typeof(object));

            _subscriptionCreationTask = new ServiceSubscriptionCreationTask<TestService, object>(
                (o, o1) => { },
                _subscriptionsFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
            _subscriptionsFactoryMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldGetServiceAndCreateSubscription()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            _subscriptionsFactoryMock
                .Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()))
                .Returns(_subscription)
                .Verifiable();

            var subscription = _subscriptionCreationTask.CreateSubscription(_appServiceProviderMock.Object);

            Assert.That(subscription, Is.Not.Null);
        }

        [Test]
        public void CreateSubscription_WhenServiceIsNotFound_ShouldThrow()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(null)
                .Verifiable();

            Assert.That(() =>
            {
                _subscriptionCreationTask.CreateSubscription(_appServiceProviderMock.Object);
            }, Throws.TypeOf<SubscribingServiceNotFoundException>());
        }

        public class TestService { }
    }
}
