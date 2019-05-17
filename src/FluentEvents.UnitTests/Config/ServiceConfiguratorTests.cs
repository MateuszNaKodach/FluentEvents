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
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;
        private Mock<IGlobalSubscriptionCollection> _globalSubscriptionCollection;

        private ServiceConfigurator<TestService> _serviceConfigurator;

        [SetUp]
        public void SetUp()
        {
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _globalSubscriptionCollection = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);

            _serviceConfigurator = new ServiceConfigurator<TestService>(
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionCollection.Object
            );
        }

        [Test]
        public void HasScopedSubscription_ShouldConfigureScopedServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceSubscription(subscriptionAction))
                .Verifiable();

            _serviceConfigurator.HasScopedSubscription(subscriptionAction);
        }

        [Test]
        public void HasScopedSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceConfigurator.HasScopedSubscription<TestSource>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void HasGlobalSubscription_ShouldAddGlobalScopeServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            _globalSubscriptionCollection
                .Setup(x => x.AddGlobalScopeServiceSubscription(subscriptionAction))
                .Verifiable();

            _serviceConfigurator.HasGlobalSubscription(subscriptionAction);
        }

        [Test]
        public void HasGlobalSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceConfigurator.HasGlobalSubscription<TestSource>(null);
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
