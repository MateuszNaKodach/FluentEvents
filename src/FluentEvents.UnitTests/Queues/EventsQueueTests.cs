using System;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueueTests
    {
        private readonly string _eventsQueueName = nameof(_eventsQueueName);
        private EventsQueue _eventsQueue;
        private QueuedPipelineEvent _queuedPipelineEvent;

        [SetUp]
        public void SetUp()
        {
            _eventsQueue = new EventsQueue(_eventsQueueName);
            var pipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            _queuedPipelineEvent = new QueuedPipelineEvent(() => Task.CompletedTask, pipelineEvent);
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
        public void Ctor_ShouldSetName()
        {
            var eventsQueue = new EventsQueue(_eventsQueueName);

            Assert.That(eventsQueue, Has.Property(nameof(EventsQueue.Name)).EqualTo(_eventsQueueName));
        }

        [Test]
        public void Enqueue_WithNullEvent_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventsQueue.Enqueue(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Enqueue_ShouldEnqueue()
        {
            _eventsQueue.Enqueue(_queuedPipelineEvent);

            var dequeuedEvents = _eventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(1).Items);
            Assert.That(dequeuedEvents, Has.Exactly(1).Items.EqualTo(_queuedPipelineEvent));
        }

        [Test]
        public void DiscardQueuedEvents_ShouldClearQueue()
        {
            _eventsQueue.Enqueue(_queuedPipelineEvent);

            _eventsQueue.DiscardQueuedEvents();

            var dequeuedEvents = _eventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(0).Items);
        }

        [Test]
        public void DequeueAll_ShouldReturnAllQueuedEvents()
        {
            var queuedEventsCount = 4;
            for (var i = 0; i < queuedEventsCount; i++)
                _eventsQueue.Enqueue(_queuedPipelineEvent);

            var dequeuedEvents = _eventsQueue.DequeueAll().ToArray();
            Assert.That(dequeuedEvents, Has.Exactly(queuedEventsCount).Items);
        }
    }
}
