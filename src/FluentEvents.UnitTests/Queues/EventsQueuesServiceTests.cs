using System;
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
        private EventsQueuesService m_EventsQueuesService;
        private Mock<IEventsQueuesFactory> m_EventsQueuesFactory;
        private Mock<IServiceProvider> m_InternalServiceProvider1;
        private Mock<IInfrastructureEventsContext> m_EventsContextMock1;
        private Mock<IServiceProvider> m_InternalServiceProvider2;
        private Mock<IInfrastructureEventsContext> m_EventsContextMock2;
        private Mock<IPipeline> m_PipelineMock;
        private Mock<IEventsQueueNamesService> m_EventsQueueNamesServiceMock1;
        private Mock<IEventsQueueNamesService> m_EventsQueueNamesServiceMock2;
        private PipelineEvent m_PipelineEvent;
        private readonly string m_QueueName = "queueName";

        [SetUp]
        public void SetUp()
        {
            m_EventsScope = new EventsScope();
            m_InternalServiceProvider1 = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsContextMock1 = MakeEventsContext(m_InternalServiceProvider1);
            m_InternalServiceProvider2 = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsContextMock2 = MakeEventsContext(m_InternalServiceProvider2);
            m_EventsQueuesFactory = new Mock<IEventsQueuesFactory>(MockBehavior.Strict);

            m_EventsQueueNamesServiceMock1 = SetUpEventsQueuesNameService(m_InternalServiceProvider1);
            m_EventsQueueNamesServiceMock2 = SetUpEventsQueuesNameService(m_InternalServiceProvider2);
            m_EventsQueuesService = new EventsQueuesService(
                new[] { m_EventsContextMock1.Object, m_EventsContextMock2.Object },
                m_EventsQueuesFactory.Object
            );
            m_PipelineEvent = MakeNewPipelineEvent();
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
        }

        private Mock<IEventsQueueNamesService> SetUpEventsQueuesNameService(Mock<IServiceProvider> serviceProviderMock)
        {
            var eventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IEventsQueueNamesService)))
                .Returns(eventsQueueNamesServiceMock.Object)
                .Verifiable();
            return eventsQueueNamesServiceMock;
        }

        private static PipelineEvent MakeNewPipelineEvent() => new PipelineEvent("f", new object(), new object());

        [TearDown]
        public void TearDown()
        {
            m_EventsQueuesFactory.Verify();
            m_InternalServiceProvider1.Verify();
            m_EventsContextMock1.Verify();
            m_InternalServiceProvider2.Verify();
            m_EventsContextMock2.Verify();
            m_PipelineMock.Verify();
            m_EventsQueueNamesServiceMock1.Verify();
            m_EventsQueueNamesServiceMock2.Verify();
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
                    isEventsContextNull ? null : m_EventsContextMock1.Object,
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

                m_PipelineMock
                    .Setup(x => x.ProcessEventAsync(pipelineEvent, m_EventsScope))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                EnqueueEvent(pipelineEvent, queueName);
            }

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContextMock1.Object, null);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithQueueName_ShouldProcessCorrectQueue()
        {
            var pipelineEvent = MakeNewPipelineEvent();

            SetUpQueueCreation(m_QueueName);

            m_PipelineMock
                .Setup(x => x.ProcessEventAsync(pipelineEvent, m_EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventsQueuesService.EnqueueEvent(pipelineEvent, m_PipelineMock.Object);

            await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContextMock1.Object, m_QueueName);
        }

        [Test]
        public void ProcessQueuedEventsAsync_WithNonExistingQueue_ShouldThrow()
        {
            m_EventsQueueNamesServiceMock1
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
                .Verifiable();

            Assert.That(async () =>
            {
                await m_EventsQueuesService.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContextMock1.Object, m_QueueName);
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
                EnqueueEvent(m_PipelineEvent, queueName);
            }

            m_EventsQueuesService.DiscardQueuedEvents(m_EventsContextMock1.Object, null);
        }

        [Test]
        public void DiscardQueuedEvents_WithNonExistingQueue_ShouldThrow()
        {
            m_EventsQueueNamesServiceMock1
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
                .Verifiable();

            Assert.That(() =>
            {
                m_EventsQueuesService.DiscardQueuedEvents(m_EventsContextMock1.Object, m_QueueName);
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
                .Returns(m_EventsContextMock1.Object)
                .Verifiable();

            m_EventsQueueNamesServiceMock1
                .Setup(x => x.IsQueueNameExisting(m_QueueName))
                .Returns(false)
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
                .Returns(m_EventsContextMock1.Object)
                .Verifiable();

            SetUpQueueCreation(m_QueueName);

            m_EventsQueuesService.EnqueueEvent(m_PipelineEvent, m_PipelineMock.Object);
        }

        private void EnqueueEvent(PipelineEvent pipelineEvent, string queueName)
        {
            SetUpQueueCreation(queueName);

            m_EventsQueuesService.EnqueueEvent(pipelineEvent, m_PipelineMock.Object);
        }

        private void SetUpQueueCreation(string queueName)
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
                .Returns(m_EventsContextMock1.Object)
                .Verifiable();

            m_EventsQueueNamesServiceMock1
                .Setup(x => x.IsQueueNameExisting(queueName))
                .Returns(true)
                .Verifiable();
        }

        private Mock<IInfrastructureEventsContext> MakeEventsContext(Mock<IServiceProvider> serviceProviderMock)
        {
            var eventsContextMock = new Mock<IInfrastructureEventsContext>(MockBehavior.Strict);
            eventsContextMock
                .Setup(x => x.Instance)
                .Returns(serviceProviderMock.Object)
                .Verifiable();

            return eventsContextMock;
        }
    }
}
