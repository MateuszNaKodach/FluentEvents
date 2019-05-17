using System;
using System.Collections.Generic;
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
        private Mock<IEventsQueueCollection> _eventQueueCollectionMock;
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<EventsContext> _eventsContextMock;
        private Mock<IPipeline> _pipelineMock;
        private Mock<IEventsQueueNamesService> _eventsQueueNamesServiceMock;

        private EventsQueuesContext _eventsQueuesContext;
        private EventsScope _eventsScope;
        private PipelineEvent _pipelineEvent;
        private readonly string _queueName = "queueName";

        private EventsQueuesService _eventsQueuesService;

        [SetUp]
        public void SetUp()
        {
            _eventQueueCollectionMock = new Mock<IEventsQueueCollection>(MockBehavior.Strict);
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            _eventsContextMock = new Mock<EventsContext>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            _eventsQueueNamesServiceMock = new Mock<IEventsQueueNamesService>(MockBehavior.Strict);
            _eventsQueuesContext = new EventsQueuesContext();
            _eventsScope = new EventsScope(
                new[] {_eventsContextMock.Object},
                _appServiceProviderMock.Object,
                _eventQueueCollectionMock.Object
            );

            _pipelineEvent = MakeNewPipelineEvent();

            _eventsQueuesService = new EventsQueuesService(_eventsQueuesContext, _eventsQueueNamesServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _eventQueueCollectionMock.Verify();
            _appServiceProviderMock.Verify();
            _eventsContextMock.Verify();
            _pipelineMock.Verify();
            _eventsContextMock.Verify();
            _eventsQueueNamesServiceMock.Verify();
        }

        private static PipelineEvent MakeNewPipelineEvent()
            => new PipelineEvent(
                typeof(object),
                "f",
                new object(),
                new object()
            );

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

            _eventQueueCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(queues.GetEnumerator())
                .Verifiable();

            await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScope, null);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_WithQueueName_ShouldProcessCorrectQueue()
        {
            var pipelineEvent = MakeNewPipelineEvent();

            SetUpGetQueue(_queueName);

            var isNextModuleInvoked = false;

            Task InvokeNextModule()
            {
                isNextModuleInvoked = true;
                return Task.CompletedTask;
            }

            _eventsQueuesService.EnqueueEvent(_eventsScope, pipelineEvent, _queueName, InvokeNextModule);

            await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScope, _queueName);

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
                await _eventsQueuesService.ProcessQueuedEventsAsync(_eventsScope, _queueName);
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

            _eventQueueCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(queues.GetEnumerator())
                .Verifiable();

            _eventsQueuesService.DiscardQueuedEvents(_eventsScope, null);
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
                _eventsQueuesService.DiscardQueuedEvents(_eventsScope, _queueName);
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
                    isEventsScopeNull ? null : _eventsScope,
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
                _eventsQueuesService.EnqueueEvent(_eventsScope, _pipelineEvent, _queueName, () => Task.CompletedTask);
            }, Throws.TypeOf<EventsQueueNotFoundException>());
        }

        [Test]
        public void EnqueueEvent_ShouldEnqueue()
        {
            SetUpGetQueue(_queueName);

            _eventsQueuesService.EnqueueEvent(_eventsScope, _pipelineEvent, _queueName, () => Task.CompletedTask);
        }

        private void SetUpGetQueue(string queueName)
        {
            var eventsQueue = new EventsQueue(queueName);

            _eventQueueCollectionMock
                .Setup(x => x.GetOrAddEventsQueue(_eventsQueuesContext, queueName))
                .Returns(eventsQueue)
                .Verifiable();

            _eventsQueueNamesServiceMock
                .Setup(x => x.IsQueueNameExisting(queueName))
                .Returns(true)
                .Verifiable();
        }
    }
}
