using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ServiceHandlerSubscriptionCreationTaskTests
    {
        private static readonly string _eventName = nameof(_eventName);

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

            _subscriptionCreationTask = new ServiceHandlerSubscriptionCreationTask<TestService, object, object>(
                _eventName,
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
                .Setup(x => x.CreateSubscription<object>(It.Is<SubscribedHandler>(y => y.EventName == _eventName)))
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

        private class TestService : IEventHandler<object, object>
        {
            public Task HandleEventAsync(object source, object args)
            {
                throw new NotImplementedException();
            }
        }
    }
}
