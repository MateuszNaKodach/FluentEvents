using System;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
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
        public void GetQueuesFeature_ShouldGerOrAddFeatureFromEventsContext()
        {
            var feature = new EventsScopeQueuesFeature();
            Func<IScopedAppServiceProvider, IEventsScopeQueuesFeature> factory = null;

            _eventsScopeMock
                .Setup(
                    x => x.GetOrAddFeature(It.IsAny<Func<IScopedAppServiceProvider, IEventsScopeQueuesFeature>>())
                )
                .Callback<Func<IScopedAppServiceProvider, IEventsScopeQueuesFeature>>(
                    x => factory = x
                )
                .Returns(feature)
                .Verifiable();

            var returnedFeature = _eventsScopeMock.Object.GetQueuesFeature();
            var factoryFeature = factory(_scopedAppServiceProviderMock.Object);

            Assert.That(returnedFeature, Is.EqualTo(feature));
            Assert.That(factoryFeature, Is.TypeOf<EventsScopeQueuesFeature>());
        }
    }
}
