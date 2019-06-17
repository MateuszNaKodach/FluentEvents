﻿using System;
using System.Threading.Tasks;
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

        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IEventsQueuesService> _eventsQueuesServiceMock;
        private EnqueuePipelineModuleConfig _enqueuePipelineModuleConfig;

        private EnqueuePipelineModule _enqueuePipelineModule;
        private PipelineEvent _pipelineEvent;
        private PipelineContext _pipelineContext;
        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _eventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);
            _enqueuePipelineModuleConfig = new EnqueuePipelineModuleConfig
            {
                QueueName = QueueName
            };
            _pipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            _eventsScope = new EventsScope();
            _pipelineContext = new PipelineContext(_pipelineEvent, _eventsScope, _serviceProviderMock.Object);

            _enqueuePipelineModule = new EnqueuePipelineModule(_eventsQueuesServiceMock.Object);
        }

        [Test]
        public async Task InvokeAsync_ShouldEnqueuePipelineEventAndNotInvokeNextModule()
        {
            var isNextInvoked = false;

            Func<Task> invokeNextModule = null;

            _eventsQueuesServiceMock
                .Setup(x => x.EnqueueEvent(_eventsScope, _pipelineEvent, QueueName, It.IsAny<Func<Task>>()))
                .Callback<EventsScope, PipelineEvent, string, Func<Task>>((_, __, ___, func) =>
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
