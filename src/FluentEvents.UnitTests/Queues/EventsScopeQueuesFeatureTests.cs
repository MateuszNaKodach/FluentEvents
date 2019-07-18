using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsScopeQueuesFeatureTests
    {
        private Mock<IEventsContext> _eventsContextMock;

        private EventsScopeQueuesFeature _eventsScope;

        [SetUp]
        public void SetUp()
        {
            _eventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);

            _eventsScope = new EventsScopeQueuesFeature();
        }

        [Test]
        public void GetOrAddEventsQueue_WithExistingQueue_ShouldOnlyReturn()
        {
            var eventsQueue1 = _eventsScope.GetOrAddEventsQueue(_eventsContextMock.Object, "1");
            var eventsQueue2 = _eventsScope.GetOrAddEventsQueue(_eventsContextMock.Object, "1");

            var eventsQueues = _eventsScope.GetEventsQueues(_eventsContextMock.Object);

            Assert.That(eventsQueues, Has.One.Items);
            Assert.That(eventsQueue1, Is.EqualTo(eventsQueue2));
        }

        [Test]
        public void GetEventsQueues_ShouldReturnAllQueues()
        {
            _eventsScope.GetOrAddEventsQueue(_eventsContextMock.Object, "1");
            _eventsScope.GetOrAddEventsQueue(_eventsContextMock.Object, "2");

            var eventsQueues = _eventsScope.GetEventsQueues(_eventsContextMock.Object);

            Assert.That(eventsQueues, Has.Exactly(2).Items);
        }
    }
}
