using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Pipelines.Queues;
using FluentEvents.Queues;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Pipelines.Queues
{
    [TestFixture]
    public class EnqueuePipelineModuleTests
    {
        private const string QueueName = nameof(QueueName);

        private Mock<IEventsQueuesService> _eventsQueuesServiceMock;
        private Mock<IEventsScope> _eventsScopeMock;

        private EnqueuePipelineModuleConfig _enqueuePipelineModuleConfig;
        private PipelineEvent _pipelineEvent;
        private PipelineContext _pipelineContext;

        private EnqueuePipelineModule _enqueuePipelineModule;

        [SetUp]
        public void SetUp()
        {
            _eventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);

            _enqueuePipelineModuleConfig = new EnqueuePipelineModuleConfig
            {
                QueueName = QueueName
            };
            _pipelineEvent = new PipelineEvent(new object());
            _pipelineContext = new PipelineContext(_pipelineEvent, _eventsScopeMock.Object);

            _enqueuePipelineModule = new EnqueuePipelineModule(_eventsQueuesServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _eventsQueuesServiceMock.Verify();
            _eventsScopeMock.Verify();
        }

        [Test]
        public async Task InvokeAsync_ShouldEnqueuePipelineEventAndNotInvokeNextModule()
        {
            var isNextInvoked = false;

            Func<Task> invokeNextModule = null;

            _eventsQueuesServiceMock
                .Setup(x => x.EnqueueEvent(_eventsScopeMock.Object, _pipelineEvent, QueueName, It.IsAny<Func<Task>>()))
                .Callback<IEventsScope, PipelineEvent, string, Func<Task>>((_, __, ___, func) =>
                {
                    invokeNextModule = func;
                })
                .Verifiable();

            await _enqueuePipelineModule.InvokeAsync(
                _enqueuePipelineModuleConfig,
                _pipelineContext,
                context =>
                {
                    isNextInvoked = true;
                    return Task.CompletedTask;
                });

            Assert.That(invokeNextModule, Is.Not.Null);
            Assert.That(isNextInvoked, Is.False);

            await invokeNextModule();

            Assert.That(isNextInvoked, Is.True);
        }
    }
}
