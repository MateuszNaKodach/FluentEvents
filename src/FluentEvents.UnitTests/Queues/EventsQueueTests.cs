using System;
using System.Linq;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueTests
    {
        private string EventsQueueName = nameof(EventsQueueName);
        private EventsQueue m_EventsQueue;
        private QueuedPipelineEvent m_QueuedPipelineEvent;

        [SetUp]
        public void SetUp()
        {
            m_EventsQueue = new EventsQueue(EventsQueueName);
            m_QueuedPipelineEvent = new QueuedPipelineEvent();
        }

        [Test]
        public void Ctor_WithNullName_ShouldThrow()
        {
           Assert.That(() =>
           {
               var eventsQueue = new EventsQueue(null); 
           }, Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void Enqueue_WithNullEvent_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventsQueue.Enqueue(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Enqueue_ShouldEnqueue()
        {
            m_EventsQueue.Enqueue(m_QueuedPipelineEvent);

            var dequeuedEvents = m_EventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(1).Items);
            Assert.That(dequeuedEvents, Has.Exactly(1).Items.EqualTo(m_QueuedPipelineEvent));
        }

        [Test]
        public void DiscardQueuedEvents_ShouldClearQueue()
        {
            m_EventsQueue.Enqueue(m_QueuedPipelineEvent);

            m_EventsQueue.DiscardQueuedEvents();

            var dequeuedEvents = m_EventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(0).Items);
        }

        [Test]
        public void DequeueAll_ShouldReturnAllQueuedEvents()
        {
            var queuedEventsCount = 4;
            for (var i = 0; i < queuedEventsCount; i++)
                m_EventsQueue.Enqueue(m_QueuedPipelineEvent);

            var dequeuedEvents = m_EventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(queuedEventsCount).Items);
        }
    }
}
