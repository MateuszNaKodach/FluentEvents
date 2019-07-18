using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class EventsScopeSubscriptionsFeatureTests
    {
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;
        private Mock<IServiceProvider> _internalServiceProviderMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;
        private Mock<IEventsContext> _eventsContextMock;

        private EventsScopeSubscriptionsFeature _eventsScopeSubscriptionsFeature;

        [SetUp]
        public void SetUp()
        {
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _eventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);

            _eventsScopeSubscriptionsFeature = new EventsScopeSubscriptionsFeature(_scopedAppServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _scopedAppServiceProviderMock.Verify();
            _internalServiceProviderMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
            _eventsContextMock.Verify();
        }

        [Test]
        public void GetSubscriptions_OnFirstCall_ShouldCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_eventsContextMock.Object).ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
        }

        [Test]
        public void GetSubscriptions_OnSecondCall_ShouldNotCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_eventsContextMock.Object).ToArray();

            var storedSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_eventsContextMock.Object).ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
            Assert.That(storedSubscriptions, Is.EquivalentTo(createdSubscriptions));
        }


        private IEnumerable<Subscription> SetUpSubscriptionsCreation()
        {
            Action<object> handler = e => { };

            var scopedSubscriptions = new[]
            {
                new Subscription(typeof(object), handler),
                new Subscription(typeof(object), handler)
            };

            _eventsContextMock
                .As<IInfrastructure<IServiceProvider>>()
                .Setup(x => x.Instance)
                .Returns(_internalServiceProviderMock.Object);

            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(_scopedSubscriptionsServiceMock.Object)
                .Verifiable();

            _scopedSubscriptionsServiceMock
                .Setup(x => x.SubscribeServices(_scopedAppServiceProviderMock.Object))
                .Returns(scopedSubscriptions)
                .Verifiable();

            return scopedSubscriptions;
        }
    }
}
