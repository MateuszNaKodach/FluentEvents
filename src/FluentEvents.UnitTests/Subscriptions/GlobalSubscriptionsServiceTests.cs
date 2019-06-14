using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class GlobalSubscriptionsServiceTests
    {
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<ISubscriptionsFactory> _subscriptionsFactoryMock;

        private GlobalSubscriptionsService _globalSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            _subscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);

            _globalSubscriptionsService = new GlobalSubscriptionsService(
                _subscriptionsFactoryMock.Object,
                _appServiceProviderMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _subscriptionsFactoryMock.Verify();
            _appServiceProviderMock.Verify();
        }

        [Test]
        public void AddGlobalSubscription_ShouldCreateSubscriptionAndAdd()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            var returnedSubscription = _globalSubscriptionsService
                .AddGlobalSubscription(action);

            Assert.That(subscription, Is.EqualTo(returnedSubscription));
            Assert.That(_globalSubscriptionsService.GetGlobalSubscriptions(), Has.One.Items);
            Assert.That(_globalSubscriptionsService.GetGlobalSubscriptions(), Has.One.Items.EqualTo(returnedSubscription));
        }

        [Test]
        public void AddGlobalServiceSubscription_ShouldEnqueueSubscriptionCreation()
        {
            _globalSubscriptionsService.AddGlobalServiceSubscription<TestService, object>((x, y) => { });
        }

        [Test]
        public void AddGlobalServiceHandlerSubscription_ShouldEnqueueSubscriptionCreation()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TestService, object, object>("");
        }

        [Test]
        public void RemoveGlobalSubscription_ShouldRemove()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            _globalSubscriptionsService
                .AddGlobalSubscription(action);

            _globalSubscriptionsService.RemoveGlobalSubscription(subscription);

            Assert.That(_globalSubscriptionsService.GetGlobalSubscriptions(), Is.Empty);
        }

        [Test]
        public void GetGlobalSubscriptions_ShouldCreateAndReturnQueuedServiceSubscriptionCreations()
        {
            _globalSubscriptionsService.AddGlobalServiceSubscription<TestService, object>((x, y) => {});
            SetUpSubscriptionsFactory(false);

            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            var subscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(subscriptions, Has.One.Items);

            var secondCallSubscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(secondCallSubscriptions, Has.One.Items);
        }

        private (Action<object>, Subscription) SetUpSubscriptionsFactory(bool isActionMatchable)
        {
            Action<object> action = x => { };
            var subscription = new Subscription(typeof(object));
            var setup = isActionMatchable 
                    ? _subscriptionsFactoryMock.Setup(x => x.CreateSubscription(action))
                    : _subscriptionsFactoryMock.Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()));

            setup
                .Returns(subscription)
                .Verifiable();

            return (action, subscription);
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
