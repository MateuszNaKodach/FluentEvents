using System;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsScopeQueuesFeatureTests
    {
        private Guid _contextGuid;

        private EventsScopeQueuesFeature _eventsScope;

        [SetUp]
        public void SetUp()
        {
            _contextGuid = Guid.NewGuid();

            _eventsScope = new EventsScopeQueuesFeature();
        }

        [Test]
        public void GetOrAddEventsQueue_WithExistingQueue_ShouldOnlyReturn()
        {
            var eventsQueue1 = _eventsScope.GetOrAddEventsQueue(_contextGuid, "1");
            var eventsQueue2 = _eventsScope.GetOrAddEventsQueue(_contextGuid, "1");

            var eventsQueues = _eventsScope.GetEventsQueues(_contextGuid);

            Assert.That(eventsQueues, Has.One.Items);
            Assert.That(eventsQueue1, Is.EqualTo(eventsQueue2));
        }

        [Test]
        public void GetEventsQueues_ShouldReturnAllQueues()
        {
            _eventsScope.GetOrAddEventsQueue(_contextGuid, "1");
            _eventsScope.GetOrAddEventsQueue(_contextGuid, "2");

            var eventsQueues = _eventsScope.GetEventsQueues(_contextGuid);

            Assert.That(eventsQueues, Has.Exactly(2).Items);
        }
    }
}
