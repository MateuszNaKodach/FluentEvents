using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueCollectionTests
    {
        private const string QueueName1 = nameof(QueueName1);
        private const string QueueName2 = nameof(QueueName2);

        private EventsQueuesContext m_EventsQueuesContext1;
        private EventsQueuesContext m_EventsQueuesContext2;

        private EventsQueueCollection m_EventsQueueCollection;

        [SetUp]
        public void SetUp()
        {
            m_EventsQueuesContext1 = new EventsQueuesContext();
            m_EventsQueuesContext2 = new EventsQueuesContext();

            m_EventsQueueCollection = new EventsQueueCollection();
        }

        [Test]
        public void GetOrAddEventsQueue_ShouldNotAddDuplicates()
        {
            var eventsQueue1 = m_EventsQueueCollection.GetOrAddEventsQueue(m_EventsQueuesContext1, QueueName1);
            var eventsQueue2 = m_EventsQueueCollection.GetOrAddEventsQueue(m_EventsQueuesContext1, QueueName1);

            Assert.That(eventsQueue1, Is.EqualTo(eventsQueue2));
        }

        [Test]
        [Sequential]
        public void GetOrAddEventsQueue_ShouldAddIfEventsQueuesContextOrQueueNameIsDifferent(
            [Values(false, true)] bool isEventsQueueContextDifferent,
            [Values(true, false)] bool isQueueNameDifferent
        )
        {
            var eventsQueue1 = m_EventsQueueCollection.GetOrAddEventsQueue(m_EventsQueuesContext1, QueueName1);

            var eventsQueueContext = isEventsQueueContextDifferent ? m_EventsQueuesContext2 : m_EventsQueuesContext1;
            var queueName = isQueueNameDifferent ? QueueName2 : QueueName1;

            var eventsQueue2 = m_EventsQueueCollection.GetOrAddEventsQueue(eventsQueueContext, queueName);

            Assert.That(eventsQueue1, Is.Not.EqualTo(eventsQueue2));
        }

        [Test]
        public void GetEnumerator_ShouldReturnEnumerator()
        {
            var enumerator = m_EventsQueueCollection.GetEnumerator();

            Assert.That(enumerator, Is.Not.Null);
        }
    }
}
