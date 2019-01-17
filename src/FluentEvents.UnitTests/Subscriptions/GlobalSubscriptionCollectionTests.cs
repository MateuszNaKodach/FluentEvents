using System;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class GlobalSubscriptionCollectionTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISubscriptionsFactory> m_SubscriptionsFactoryMock;

        private GlobalSubscriptionCollection m_GlobalSubscriptionCollection;

        [SetUp]
        public void SetUp()
        {
            m_SubscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);

            m_GlobalSubscriptionCollection = new GlobalSubscriptionCollection(
                m_SubscriptionsFactoryMock.Object,
                m_ServiceProviderMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_SubscriptionsFactoryMock.Verify();
            m_ServiceProviderMock.Verify();
        }

        [Test]
        public void AddGlobalScopeSubscription_ShouldCreateSubscriptionAndAdd()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            var returnedSubscription = m_GlobalSubscriptionCollection
                .AddGlobalScopeSubscription(action);

            Assert.That(subscription, Is.EqualTo(returnedSubscription));
            Assert.That(m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions(), Has.One.Items);
            Assert.That(m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions(), Has.One.Items.EqualTo(returnedSubscription));
        }

        [Test]
        public void AddGlobalScopeServiceSubscription_ShouldEnqueueSubscriptionCreation()
        {
            m_GlobalSubscriptionCollection.AddGlobalScopeServiceSubscription<object, object>((x, y) => { });
        }

        [Test]
        public void RemoveGlobalScopeSubscription_ShouldRemove()
        {
            var (action, subscription) = SetUpSubscriptionsFactory(true);

            m_GlobalSubscriptionCollection
                .AddGlobalScopeSubscription(action);

            m_GlobalSubscriptionCollection.RemoveGlobalScopeSubscription(subscription);

            Assert.That(m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions(), Is.Empty);
        }

        [Test]
        public void GetGlobalScopeSubscriptions_ShouldCreateAndReturnQueuedServiceSubscriptionCreations()
        {
            m_GlobalSubscriptionCollection.AddGlobalScopeServiceSubscription<TestService, object>((x, y) => {});
            SetUpSubscriptionsFactory(false);

            m_ServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            var subscriptions = m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions();
            Assert.That(subscriptions, Has.One.Items);

            var secondCallSubscriptions = m_GlobalSubscriptionCollection.GetGlobalScopeSubscriptions();
            Assert.That(secondCallSubscriptions, Has.One.Items);
        }

        private (Action<object>, Subscription) SetUpSubscriptionsFactory(bool isActionMatchable)
        {
            Action<object> action = x => { };
            var subscription = new Subscription(typeof(object));
            var setup = isActionMatchable 
                    ? m_SubscriptionsFactoryMock.Setup(x => x.CreateSubscription(action))
                    : m_SubscriptionsFactoryMock.Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()));

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
