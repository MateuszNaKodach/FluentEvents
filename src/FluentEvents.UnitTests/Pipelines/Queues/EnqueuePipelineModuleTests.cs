using System;
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

        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<IEventsQueuesService> m_EventsQueuesServiceMock;
        private EnqueuePipelineModuleConfig m_EnqueuePipelineModuleConfig;

        private EnqueuePipelineModule m_EnqueuePipelineModule;
        private PipelineEvent m_PipelineEvent;
        private PipelineContext m_PipelineContext;
        private EventsScope m_EventsScope;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);
            m_EnqueuePipelineModuleConfig = new EnqueuePipelineModuleConfig
            {
                QueueName = QueueName
            };
            m_PipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            m_EventsScope = new EventsScope();
            m_PipelineContext = new PipelineContext(m_PipelineEvent, m_EventsScope, m_ServiceProviderMock.Object);

            m_EnqueuePipelineModule = new EnqueuePipelineModule(m_EventsQueuesServiceMock.Object);
        }

        [Test]
        public async Task InvokeAsync_ShouldEnqueuePipelineEventAndNotInvokeNextModule()
        {
            var isNextInvoked = false;

            m_EventsQueuesServiceMock
                .Setup(x => x.EnqueueEvent(m_EventsScope, m_PipelineEvent, QueueName, It.IsAny<Func<Task>>()))
                .Verifiable();

            await m_EnqueuePipelineModule.InvokeAsync(
                m_EnqueuePipelineModuleConfig,
                m_PipelineContext,
                context =>
                {
                    isNextInvoked = true;
                    return Task.CompletedTask;
                });

            Assert.That(isNextInvoked, Is.False);
        }
    }
}
