using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Queues
{
    [TestFixture]
    public class EventsQueuesServiceTests
    {
        private static readonly string _queueName = "queueName";

        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<EventsContext> _eventsContextMock;
        private Mock<IPipeline> _pipelineMock;
        private Mock<IEventsQueueNamesService> _eventsQueueNamesServiceMock;
        private Mock<IEventsScope> _eventsScopeMock;

        private EventsQueue _eventsQueue;
        private PipelineEvent _pipelineEvent;

        private EventsQueuesService _eventsQueuesService;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            _eventsContextMock = new Mock<EventsContext>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _eventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);

            _eventsQueue = new EventsQueue(_queueName);
            _pipelineEvent = MakeNewPipelineEvent();

            _eventsQueuesService = new EventsQueuesService(_eventsQueueNamesServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
            _eventsContextMock.Verify();
            _pipelineMock.Verify();
            _eventsContextMock.Verify();
            _eventsQueueNamesServiceMock.Verify();
        }

        private static PipelineEvent MakeNewPipelineEvent()
            => new PipelineEvent(typeof(object));

        [Test]
        [Sequential]
        public void ProcessQueuedEventsAsync_WithNullEventsScope_ShouldThrow()
        {
            Assert.That(async () =>
            {
                await _eventsQueuesService.ProcessQueuedEventsAsync(null, null);
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

            _eventsScopeMock
                .Setup(x => x.GetEventsQueues())
                .Returns(queues)
                .Verifiable();

            await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScopeMock.Object, null);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithQueueName_ShouldProcessCorrectQueue()
        {
            var pipelineEvent = MakeNewPipelineEvent();

            SetUpGetQueue();

            var isNextModuleInvoked = false;

            Task InvokeNextModule()
            {
                isNextModuleInvoked = true;
                return Task.CompletedTask;
            }

            _eventsQueuesService.EnqueueEvent(_eventsScopeMock.Object, pipelineEvent, _queueName, InvokeNextModule);

            await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScopeMock.Object, _queueName);

            Assert.That(isNextModuleInvoked, Is.True);
        }

        [Test]
        public void ProcessQueuedEventsAsync_WithNonExistingQueue_ShouldThrow()
        {
            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(_queueName))
                .Returns(false)
                .Verifiable();

            Assert.That(async () =>
            {
                await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScopeMock.Object, _queueName);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void DiscardQueuedEvents_ShouldThrow()
        {
            Assert.That(() =>
            {
                _eventsQueuesService.DiscardQueuedEvents(null, null);
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

            _eventsScopeMock
                .Setup(x => x.GetEventsQueues())
                .Returns(queues)
                .Verifiable();

            _eventsQueuesService.DiscardQueuedEvents(_eventsScopeMock.Object, null);
        }

        [Test]
        public void DiscardQueuedEvents_WithQueueName_ShouldClearSpecificQueue()
        {
            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(_queueName))
                .Returns(true)
                .Verifiable();

            _eventsScopeMock
                .Setup(x => x.GetOrAddEventsQueue(_queueName))
                .Returns(_eventsQueue)
                .Verifiable();

            _eventsQueue.Enqueue(new QueuedPipelineEvent(null, null));

            _eventsQueuesService.DiscardQueuedEvents(_eventsScopeMock.Object, _queueName);

            Assert.That(_eventsQueue.DequeueAll(), Has.Exactly(0).Items);
        }

        [Test]
        public void DiscardQueuedEvents_WithNonExistingQueue_ShouldThrow()
        {
            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(_queueName))
                .Returns(false)
                .Verifiable();

            Assert.That(() =>
            {
                _eventsQueuesService.DiscardQueuedEvents(_eventsScopeMock.Object, _queueName);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        [Sequential]
        public void EnqueueEvent_WithNullArgs_ShouldThrow(
            [Values(true, false, false)] bool isEventsScopeNull,
            [Values(false, true, false)] bool isPipelineEventNull,
            [Values(false, false, true)] bool isQueueNameNull
        )
        {
            Assert.That(() =>
            {
                _eventsQueuesService.EnqueueEvent(
                    isEventsScopeNull ? null : _eventsScopeMock.Object,
                    isPipelineEventNull ? null : _pipelineEvent,
                    isQueueNameNull ? null : _queueName,
                    () => Task.CompletedTask
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EnqueueEvent_WithNonExistingQueue_ShouldThrow()
        {
            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(_queueName))
                .Returns(false)
                .Verifiable();

            Assert.That(() =>
            {
                _eventsQueuesService.EnqueueEvent(_eventsScopeMock.Object, _pipelineEvent, _queueName, () => Task.CompletedTask);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void EnqueueEvent_ShouldEnqueue()
        {
            SetUpGetQueue();

            Func<Task> invokeNextModule = () => Task.CompletedTask;
            _eventsQueuesService.EnqueueEvent(_eventsScopeMock.Object, _pipelineEvent, _queueName, invokeNextModule);

            var queuedPipelineEvents = _eventsQueue.DequeueAll().ToList();

            Assert.That(queuedPipelineEvents, Has.One.Items);
            Assert.That(queuedPipelineEvents, Has.One.Items.With.Property(nameof(QueuedPipelineEvent.PipelineEvent)).EqualTo(_pipelineEvent));
            Assert.That(queuedPipelineEvents, Has.One.Items.With.Property(nameof(QueuedPipelineEvent.InvokeNextModule)).EqualTo(invokeNextModule));
        }

        private void SetUpGetQueue()
        {
            _eventsScopeMock
                .Setup(x => x.GetOrAddEventsQueue(_queueName))
                .Returns(_eventsQueue)
                .Verifiable();

            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(_queueName))
                .Returns(true)
                .Verifiable();
        }
    }
}
