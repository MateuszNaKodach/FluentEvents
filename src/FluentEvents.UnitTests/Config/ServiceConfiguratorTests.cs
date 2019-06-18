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
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsService;

        private ServiceConfigurator<TestService> _serviceConfigurator;

        [SetUp]
        public void SetUp()
        {
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _globalSubscriptionsService = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);

            _serviceConfigurator = new ServiceConfigurator<TestService>(
                _scopedSubscriptionsServiceMock.Object,
                _globalSubscriptionsService.Object
            );
        }

        [Test]
        public void HasScopedSubscription_ShouldConfigureScopedServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            _scopedSubscriptionsServiceMock
                .Setup(x => x.ConfigureScopedServiceSubscription(subscriptionAction))
                .Verifiable();

            _serviceConfigurator.HasScopedSubscriptionTo(subscriptionAction);
        }

        [Test]
        public void HasScopedSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceConfigurator.HasScopedSubscriptionTo<TestSource>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void HasGlobalSubscription_ShouldAddGlobalScopeServiceSubscription()
        {
            Action<TestService, TestSource> subscriptionAction = (service, source) => { };

            _globalSubscriptionsService
                .Setup(x => x.AddGlobalServiceSubscription(subscriptionAction))
                .Verifiable();

            _serviceConfigurator.HasGlobalSubscriptionTo(subscriptionAction);
        }

        [Test]
        public void HasGlobalSubscription_WithNullSubscriptionAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _serviceConfigurator.HasGlobalSubscriptionTo<TestSource>(null);
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
