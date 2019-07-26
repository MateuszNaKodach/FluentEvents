using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class EventsScopeSubscriptionsFeatureTests
    {
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;

        private EventsScopeSubscriptionsFeature _eventsScopeSubscriptionsFeature;

        [SetUp]
        public void SetUp()
        {
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _eventsScopeSubscriptionsFeature = new EventsScopeSubscriptionsFeature(_scopedAppServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _scopedAppServiceProviderMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
        }

        [Test]
        public void GetSubscriptions_OnFirstCall_ShouldCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_scopedSubscriptionsServiceMock.Object).ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
        }

        [Test]
        public void GetSubscriptions_OnSecondCall_ShouldNotCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_scopedSubscriptionsServiceMock.Object).ToArray();

            var storedSubscriptions = _eventsScopeSubscriptionsFeature.GetSubscriptions(_scopedSubscriptionsServiceMock.Object).ToArray();

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

            _scopedSubscriptionsServiceMock
                .Setup(x => x.SubscribeServices(_scopedAppServiceProviderMock.Object))
                .Returns(scopedSubscriptions)
                .Verifiable();

            return scopedSubscriptions;
        }
    }
}
