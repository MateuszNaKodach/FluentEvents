using System.Collections;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueCollectionTests
    {
        private const string QueueName1 = nameof(QueueName1);
        private const string QueueName2 = nameof(QueueName2);

        private EventsQueuesContext _eventsQueuesContext1;
        private EventsQueuesContext _eventsQueuesContext2;

        private EventsQueueCollection _eventsQueueCollection;

        [SetUp]
        public void SetUp()
        {
            _eventsQueuesContext1 = new EventsQueuesContext();
            _eventsQueuesContext2 = new EventsQueuesContext();

            _eventsQueueCollection = new EventsQueueCollection();
        }

        [Test]
        public void GetOrAddEventsQueue_ShouldNotAddDuplicates()
        {
            var eventsQueue1 = _eventsQueueCollection.GetOrAddEventsQueue(_eventsQueuesContext1, QueueName1);
            var eventsQueue2 = _eventsQueueCollection.GetOrAddEventsQueue(_eventsQueuesContext1, QueueName1);

            Assert.That(eventsQueue1, Is.EqualTo(eventsQueue2));
        }

        [Test]
        [Sequential]
        public void GetOrAddEventsQueue_ShouldAddIfEventsQueuesContextOrQueueNameIsDifferent(
            [Values(false, true)] bool isEventsQueueContextDifferent,
            [Values(true, false)] bool isQueueNameDifferent
        )
        {
            var eventsQueue1 = _eventsQueueCollection.GetOrAddEventsQueue(_eventsQueuesContext1, QueueName1);

            var eventsQueueContext = isEventsQueueContextDifferent ? _eventsQueuesContext2 : _eventsQueuesContext1;
            var queueName = isQueueNameDifferent ? QueueName2 : QueueName1;

            var eventsQueue2 = _eventsQueueCollection.GetOrAddEventsQueue(eventsQueueContext, queueName);

            Assert.That(eventsQueue1, Is.Not.EqualTo(eventsQueue2));
        }

        [Test]
        public void GetEnumerator_ShouldReturnEnumerator()
        {
            var enumerator = _eventsQueueCollection.GetEnumerator();

            Assert.That(enumerator, Is.Not.Null);
        }

        [Test]
        public void ExplicitGetEnumerator_ShouldReturnEnumerator()
        {
            var enumerator = ((IEnumerable)_eventsQueueCollection).GetEnumerator();

            Assert.That(enumerator, Is.Not.Null);
        }
    }
}
