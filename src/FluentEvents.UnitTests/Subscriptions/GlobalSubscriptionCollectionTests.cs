using System;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class GlobalSubscriptionCollectionTests
    {
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<ISubscriptionsFactory> _subscriptionsFactoryMock;

        private GlobalSubscriptionCollection _globalSubscriptionCollection;

        [SetUp]
        public void SetUp()
        {
            _subscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);

            _globalSubscriptionCollection = new GlobalSubscriptionCollection(
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
        public void AddGlobalScopeSubscription_ShouldCreateSubscriptionAndAdd()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            var returnedSubscription = _globalSubscriptionCollection
                .AddGlobalScopeSubscription(action);

            Assert.That(subscription, Is.EqualTo(returnedSubscription));
            Assert.That(_globalSubscriptionCollection.GetGlobalScopeSubscriptions(), Has.One.Items);
            Assert.That(_globalSubscriptionCollection.GetGlobalScopeSubscriptions(), Has.One.Items.EqualTo(returnedSubscription));
        }

        [Test]
        public void AddGlobalScopeServiceSubscription_ShouldEnqueueSubscriptionCreation()
        {
            _globalSubscriptionCollection.AddGlobalScopeServiceSubscription<object, object>((x, y) => { });
        }

        [Test]
        public void RemoveGlobalScopeSubscription_ShouldRemove()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            _globalSubscriptionCollection
                .AddGlobalScopeSubscription(action);

            _globalSubscriptionCollection.RemoveGlobalScopeSubscription(subscription);

            Assert.That(_globalSubscriptionCollection.GetGlobalScopeSubscriptions(), Is.Empty);
        }

        [Test]
        public void GetGlobalScopeSubscriptions_ShouldCreateAndReturnQueuedServiceSubscriptionCreations()
        {
            _globalSubscriptionCollection.AddGlobalScopeServiceSubscription<TestService, object>((x, y) => {});
            SetUpSubscriptionsFactory(false);

            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            var subscriptions = _globalSubscriptionCollection.GetGlobalScopeSubscriptions();
            Assert.That(subscriptions, Has.One.Items);

            var secondCallSubscriptions = _globalSubscriptionCollection.GetGlobalScopeSubscriptions();
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

        private class TestService
        {
        }
    }
}
