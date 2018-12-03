using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueuesServiceTests
    {
        private EventsScope m_EventsScope;
        private EventsQueuesService m_EventsQueuesService;
        private Mock<IEventsQueuesFactory> m_EventsQueuesFactory;
        private EventsContextImpl m_EventsContext;
        private EventsContextImpl m_EventsContext2;
        private PipelineEvent m_PipelineEvent;
        private Mock<IPipeline> m_PipelineMock;
        private readonly string m_QueueName = "queueName";

        [SetUp]
        public void SetUp()
        {
            m_EventsScope = new EventsScope();
            m_EventsContext = new EventsContextImpl();
            m_EventsContext2 = new EventsContextImpl();
            m_EventsQueuesFactory = new Mock<IEventsQueuesFactory>(MockBehavior.Strict);
            m_EventsQueuesService = new EventsQueuesService(m_EventsQueuesFactory.Object);
            m_PipelineEvent = MakeNewPipelineEvent();
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
        }

        private static PipelineEvent MakeNewPipelineEvent() => new PipelineEvent("f", new object(), new object());

        [TearDown]
        public void TearDown()
        {
            m_EventsQueuesFactory.Verify();
            m_PipelineMock.Verify();
        }

        [Test]
        [Sequential]
        public void CreateQueueIfNotExists_WithNullArgs_ShouldThrow(
            [Values(true, true, false)] bool isQueueNameNull,
            [Values(true, false, true)] bool isEventsContextNull
        )
        {
            Assert.That(() =>
            {
                m_EventsQueuesService.CreateQueueIfNotExists(
                    isEventsContextNull ? null : m_EventsContext,
                    isQueueNameNull ? null : m_QueueName
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CreateQueueIfNotExists_WhenCalledTwiceWithSameEventsContextAndQueueName_ShouldCreateOneQueue()
        {
            m_EventsQueuesFactory
                .Setup(x => x.GetNew(m_QueueName))
                .Returns(new EventsQueue(m_QueueName))
                .Verifiable();

            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, m_QueueName);
            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, m_QueueName);

            Assert.That(m_EventsQueuesFactory, Has.Property(nameof(Mock.Invocations)).With.One.Items);
        }

        [Test]
        public void CreateQueueIfNotExists_WhenCalledTwiceWithDifferentEventsContextAndSameQueueName_ShouldCreateTwoQueues()
        {
            m_EventsQueuesFactory
                .Setup(x => x.GetNew(m_QueueName))
                .Returns(new EventsQueue(m_QueueName))
                .Verifiable();

            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, m_QueueName);
            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext2, m_QueueName);

            Assert.That(m_EventsQueuesFactory, Has.Property(nameof(Mock.Invocations)).With.Exactly(2).Items);
        }


        [Test]
        public void CreateQueueIfNotExists_WhenCalledTwiceWithSameEventsContextAndDifferentQueueNames_ShouldCreateTwoQueues()
        {
            m_EventsQueuesFactory
                .Setup(x => x.GetNew(m_QueueName))
                .Returns(new EventsQueue(m_QueueName))
                .Verifiable();

            var queue2Name = m_QueueName + "2";
            m_EventsQueuesFactory
                .Setup(x => x.GetNew(queue2Name))
                .Returns(new EventsQueue(queue2Name))
                .Verifiable();

            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, m_QueueName);
            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, queue2Name);

            Assert.That(m_EventsQueuesFactory, Has.Property(nameof(Mock.Invocations)).With.Exactly(2).Items);
        }

        [Test]
        [Sequential]
        public void ProcessQueuedEventsAsync_WithNullArgs_ShouldThrow(
            [Values(true, true, false)] bool isEventsScopeNull,
            [Values(true, false, true)] bool isEventsContextNull
        )
        {
            Assert.That(async () =>
            {
                await m_EventsQueuesService.ProcessQueuedEventsAsync(
                    isEventsScopeNull ? null : m_EventsScope,
                    isEventsContextNull ? null : m_EventsContext,
                    null
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithNullQueueName_ShouldProcessAllQueues()
        {
            for (var i = 0; i < 5; i++)
            {
                var pipelineEvent = MakeNewPipelineEvent();
                var queueName = i.ToString();

                SetUpQueue(queueName);
                m_PipelineMock
                    .Setup(x => x.ProcessEventAsync(pipelineEvent, m_EventsScope))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                m_EventsQueuesService.EnqueueEvent(pipelineEvent, m_PipelineMock.Object);
            }

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContext, null);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithQueueName_ShouldProcessCorrectQueue()
        {
            var pipelineEvent = MakeNewPipelineEvent();

            SetUpQueue(m_QueueName);

            m_PipelineMock
                .Setup(x => x.ProcessEventAsync(pipelineEvent, m_EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventsQueuesService.EnqueueEvent(pipelineEvent, m_PipelineMock.Object);

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContext, m_QueueName);
        }

        [Test]
        public void ProcessQueuedEventsAsync_WithNonExistingQueue_ShouldThrow()
        {
            Assert.That(async () =>
            {
                await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContext, m_QueueName);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void DiscardQueuedEvents_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventsQueuesService.DiscardQueuedEvents(null, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DiscardQueuedEvents_WithNullQueueName_ShouldClearAllQueues()
        {
            for (var i = 0; i < 2; i++)
            {
                var eventsQueueMock = new Mock<IEventsQueue>(MockBehavior.Strict);
                eventsQueueMock
                    .Setup(x => x.DiscardQueuedEvents())
                    .Verifiable();

                var queueName = i.ToString();
                m_EventsQueuesFactory
                    .Setup(x => x.GetNew(queueName))
                    .Returns(eventsQueueMock.Object)
                    .Verifiable();

                m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, queueName);
            }

            m_EventsQueuesService.DiscardQueuedEvents(m_EventsContext, null);
        }

        [Test]
        public void DiscardQueuedEvents_WithNonExistingQueue_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_EventsQueuesService.DiscardQueuedEvents(m_EventsContext, "notExistingQueue");
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        [Sequential]
        public void EnqueueEvent_WithNullArgs_ShouldThrow(
            [Values(true, true, false)] bool isEventsScopeNull,
            [Values(true, false, true)] bool isEventsContextNull
        )
        {
            Assert.That(() =>
            {
                m_EventsQueuesService.EnqueueEvent(
                    isEventsScopeNull ? null : m_PipelineEvent,
                    isEventsContextNull ? null : m_PipelineMock.Object
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EnqueueEvent_WithNonExistingQueue_ShouldThrow()
        {
            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns(m_QueueName)
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.EventsContext)
                .Returns(m_EventsContext)
                .Verifiable();

            Assert.That(() =>
            {
                m_EventsQueuesService.EnqueueEvent(m_PipelineEvent, m_PipelineMock.Object);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void EnqueueEvent_ShouldEnqueue()
        {
            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns(m_QueueName)
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.EventsContext)
                .Returns(m_EventsContext)
                .Verifiable();

            SetUpQueue(m_QueueName);

            m_EventsQueuesService.EnqueueEvent(m_PipelineEvent, m_PipelineMock.Object);
        }

        private void SetUpQueue(string queueName)
        {
            m_EventsQueuesFactory
                .Setup(x => x.GetNew(queueName))
                .Returns(new EventsQueue(queueName))
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns(queueName)
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.EventsContext)
                .Returns(m_EventsContext)
                .Verifiable();

            m_EventsQueuesService.CreateQueueIfNotExists(m_EventsContext, queueName);
        }
    }
}
