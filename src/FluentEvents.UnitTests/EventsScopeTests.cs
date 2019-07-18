using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class EventsScopeTests
    {
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;

        private IEventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);

            _eventsScope = new EventsScope(_scopedAppServiceProviderMock.Object);
        }

        [Test]
        public void GetOrAddFeature_WithExistingFeature_ShouldInvokeFactoryOnlyFirstTimeAndReturnTheSameInstance()
        {
            var feature = new object();
            object Factory(IScopedAppServiceProvider x) => feature;
            var feature1 = _eventsScope.GetOrAddFeature(Factory);
            var feature2 = _eventsScope.GetOrAddFeature(x => new object());

            Assert.That(feature, Is.EqualTo(feature1).And.EqualTo(feature2));
        }
    }
}
