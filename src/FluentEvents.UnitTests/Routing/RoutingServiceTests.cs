using System;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using FluentEvents.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Routing
{
    [TestFixture]
    public class RoutingServiceTests
    {
        private Mock<ILogger<RoutingService>> m_LoggerMock;
        private Mock<IDisposable> m_LoggerScopeMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IPipeline> m_PipelineMock;
        private Mock<IEventsQueuesService> m_EventsQueuesServiceMock;

        private EventsScope m_EventsScope;
        private RoutingService m_RoutingService;
        private PipelineEvent m_PipelineEvent;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;

        private readonly string m_EventFieldName = nameof(TestSource.TestEvent);

        [SetUp]
        public void SetUp()
        {
            m_LoggerMock = new Mock<ILogger<RoutingService>>(MockBehavior.Strict);
            m_LoggerScopeMock = new Mock<IDisposable>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            m_EventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);

            m_EventsScope = new EventsScope();
            m_RoutingService = new RoutingService(
                m_LoggerMock.Object,
                m_SourceModelsServiceMock.Object,
                m_EventsQueuesServiceMock.Object
            );
            m_PipelineEvent = new PipelineEvent(
                typeof(TestSource),
                m_EventFieldName,
                new TestSource(),
                new TestEventArgs()
            );
            m_SourceModel = new SourceModel(typeof(TestSource));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(m_EventFieldName);
        }

        [TearDown]
        public void TearDown()
        {
            m_LoggerMock.Verify();
            m_LoggerScopeMock.Verify();
            m_SourceModelsServiceMock.Verify();
            m_PipelineMock.Verify();
            m_EventsQueuesServiceMock.Verify();
        }

        [Test]
        public async Task RouteEventAsync_WithoutQueue_ShouldProcessEvent()
        {
            m_PipelineMock
                .Setup(x => x.ProcessEventAsync(m_PipelineEvent, m_EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_PipelineMock
                .Setup(x => x.QueueName)
                .Returns<string>(null)
                .Verifiable();

            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(m_PipelineEvent.OriginalSender.GetType()))
                .Returns(m_SourceModel)
                .Verifiable();

            m_LoggerScopeMock
                .Setup(x => x.Dispose())
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.BeginScope(It.IsAny<object>()))
                .Returns(m_LoggerScopeMock.Object)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    RoutingLoggerMessages.EventIds.EventRoutedToPipeline,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            m_SourceModelEventField
                .AddPipeline(m_PipelineMock.Object);

            await m_RoutingService.RouteEventAsync(m_PipelineEvent, m_EventsScope);
        }

        private class TestSource
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestEventArgs
        {
        }
    }
}
