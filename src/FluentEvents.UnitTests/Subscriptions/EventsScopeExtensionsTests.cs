using System;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class EventsScopeExtensionsTests
    {
        private Mock<IEventsScope> _eventsScopeMock;
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;

        [SetUp]
        public void SetUp()
        {
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _eventsScopeMock.Verify();
            _scopedAppServiceProviderMock.Verify();
        }

        [Test]
        public void GetSubscriptionsFeature_ShouldGerOrAddFeatureFromEventsContext()
        {
            var feature = new EventsScopeSubscriptionsFeature(_scopedAppServiceProviderMock.Object);
            Func<IScopedAppServiceProvider, IEventsScopeSubscriptionsFeature> factory = null;

            _eventsScopeMock
                .Setup(
                    x => x.GetOrAddFeature(It.IsAny<Func<IScopedAppServiceProvider, IEventsScopeSubscriptionsFeature>>())
                )
                .Callback<Func<IScopedAppServiceProvider, IEventsScopeSubscriptionsFeature>>(
                    x => factory = x
                )
                .Returns(feature)
                .Verifiable();

            var returnedFeature = _eventsScopeMock.Object.GetSubscriptionsFeature();
            var factoryFeature = factory(_scopedAppServiceProviderMock.Object);

            Assert.That(returnedFeature, Is.EqualTo(feature));
            Assert.That(factoryFeature, Is.TypeOf<EventsScopeSubscriptionsFeature>());
        } 
    }
}
