using System;
using FluentEvents.Config;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class ServiceConfiguratorTests
    {
        private Mock<IScopedSubscriptionsService> m_ScopedSubscriptionsServiceMock;
        private Mock<IGlobalSubscriptionCollection> m_GlobalSubscriptionCollection;

        private ServiceConfigurator<TestService> m_ServiceConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ScopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            m_GlobalSubscriptionCollection = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);

            m_ServiceConfigurator = new ServiceConfigurator<TestService>(
                m_ScopedSubscriptionsServiceMock.Object,
                m_GlobalSubscriptionCollection.Object
            );
        }

        [Test]
        public void HasScopedSubscription_ShouldConfigureScopedServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            m_ScopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceSubscription(subscriptionAction))
                .Verifiable();

            m_ServiceConfigurator.HasScopedSubscription(subscriptionAction);
        }

        [Test]
        public void HasScopedSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_ServiceConfigurator.HasScopedSubscription<TestSource>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void HasGlobalSubscription_ShouldAddGlobalScopeServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            m_GlobalSubscriptionCollection
                .Setup(x => x.AddGlobalScopeServiceSubscription(subscriptionAction))
                .Verifiable();

            m_ServiceConfigurator.HasGlobalSubscription(subscriptionAction);
        }

        [Test]
        public void HasGlobalSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_ServiceConfigurator.HasGlobalSubscription<TestSource>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        private class TestSource
        {
        }

        private class TestService
        {

        }
    }
}
