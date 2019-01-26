using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Config;
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
        private Mock<IEventQueueCollection> m_EventQueueCollectionMock;
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<IInfrastructureEventsContext> m_EventsContextMock;
        private Mock<IPipeline> m_PipelineMock;
        private Mock<IEventsQueueNamesService> m_EventsQueueNamesServiceMock;
        private PipelineEvent m_PipelineEvent;
        private readonly string m_QueueName = "queueName";

        private EventsQueuesService m_EventsQueuesService;

        [SetUp]
        public void SetUp()
        {
            m_EventQueueCollectionMock = new Mock<IEventQueueCollection>(MockBehavior.Strict);
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsContextMock = new Mock<IInfrastructureEventsContext>(MockBehavior.Strict);
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            m_EventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            m_EventsScope = new EventsScope(
                new[] {m_EventsContextMock.Object},
                m_ServiceProviderMock.Object,
                m_EventQueueCollectionMock.Object
            );

            m_PipelineEvent = MakeNewPipelineEvent();

            m_EventsQueuesService = new EventsQueuesService(m_EventsContextMock.Object, m_EventsQueueNamesServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_EventQueueCollectionMock.Verify();
            m_ServiceProviderMock.Verify();
            m_PipelineMock.Verify();
            m_EventsContextMock.Verify();
            m_EventsQueueNamesServiceMock.Verify();
        }

        private static PipelineEvent MakeNewPipelineEvent() 
            => new PipelineEvent("f", new object(), new object());

        [Test]
        [Sequential]
        public void ProcessQueuedEventsAsync_WithNullEventsScope_ShouldThrow()
        {
            Assert.That(async () =>
            {
                await m_EventsQueuesService.ProcessQueuedEventsAsync(null, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithNullQueueName_ShouldProcessAllQueues()
        {
            var queues = new List<EventsQueue>(5);

            for (var i = 0; i < 5; i++)
            {
                var queueName = i.ToString();
                queues.Add(new EventsQueue(queueName));
            }

            m_EventQueueCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(queues.GetEnumerator())
                .Verifiable();

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, null);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithQueueName_ShouldProcessCorrectQueue()
        {
            var pipelineEvent = MakeNewPipelineEvent();

            SetUpGetQueue(m_QueueName);

            m_PipelineMock
                .Setup(x => x.ProcessEventAsync(pipelineEvent, m_EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventsQueuesService.EnqueueEvent(m_EventsScope, pipelineEvent, m_PipelineMock.Object);

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_QueueName);
        }

        [Test]
        public void ProcessQueuedEventsAsync_WithNonExistingQueue_ShouldThrow()
        {
            m_EventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
                .Verifiable();

            Assert.That(async () =>
            {
                await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_QueueName);
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
            var queues = new List<EventsQueue>(5);

            for (var i = 0; i < 5; i++)
            {
                var queueName = i.ToString();
                queues.Add(new EventsQueue(queueName));
            }

            m_EventQueueCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(queues.GetEnumerator())
                .Verifiable();

            m_EventsQueuesService.DiscardQueuedEvents(m_EventsScope, null);
        }

        [Test]
        public void DiscardQueuedEvents_WithNonExistingQueue_ShouldThrow()
        {
            m_EventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
                .Verifiable();

            Assert.That(() =>
            {
                m_EventsQueuesService.DiscardQueuedEvents(m_EventsScope, m_QueueName);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        [Sequential]
        public void EnqueueEvent_WithNullArgs_ShouldThrow(
            [Values(true, false, false)] bool isEventsScopeNull,
            [Values(false, true, false)] bool isPipelineEventNull,
            [Values(false, false, true)] bool isPipelineNull
        )
        {
            Assert.That(() =>
            {
                m_EventsQueuesService.EnqueueEvent(
                    isEventsScopeNull ? null : m_EventsScope,
                    isPipelineEventNull ? null : m_PipelineEvent,
                    isPipelineNull ? null : m_PipelineMock.Object
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

            m_EventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
                .Verifiable();

            Assert.That(() =>
            {
                m_EventsQueuesService.EnqueueEvent(m_EventsScope, m_PipelineEvent, m_PipelineMock.Object);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void EnqueueEvent_ShouldEnqueue()
        {
            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns(m_QueueName)
                .Verifiable();

            SetUpGetQueue(m_QueueName);

            m_EventsQueuesService.EnqueueEvent(m_EventsScope, m_PipelineEvent, m_PipelineMock.Object);
        }

        private void EnqueueEvent(PipelineEvent pipelineEvent, string queueName)
        {
            SetUpGetQueue(queueName);

            m_EventsQueuesService.EnqueueEvent(m_EventsScope, pipelineEvent, m_PipelineMock.Object);
        }

        private EventsQueue SetUpGetQueue(string queueName)
        {
            var eventsQueue = new EventsQueue(queueName);

            m_EventQueueCollectionMock
                .Setup(x => x.GetOrAddEventsQueue(m_EventsContextMock.Object, queueName))
                .Returns(eventsQueue)
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns(queueName)
                .Verifiable();

            m_EventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(queueName))
                .Returns(true)
                .Verifiable();

            return eventsQueue;
        }
    }
}
