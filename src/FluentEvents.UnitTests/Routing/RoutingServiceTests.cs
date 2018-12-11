using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;
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
        private Mock<ITypesResolutionService> m_TypesResolutionServiceMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IPipeline> m_PipelineMock;
        private Mock<IInfrastructureEventsContext> m_EventsContextMock;

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
            m_TypesResolutionServiceMock = new Mock<ITypesResolutionService>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_PipelineMock = new Mock<IPipeline>(MockBehavior.Strict);
            m_EventsContextMock = new Mock<IInfrastructureEventsContext>(MockBehavior.Strict);

            m_EventsScope = new EventsScope();
            m_RoutingService = new RoutingService(
                m_LoggerMock.Object,
                m_TypesResolutionServiceMock.Object,
                m_SourceModelsServiceMock.Object
            );
            m_PipelineEvent = new PipelineEvent(m_EventFieldName, new TestSource(), new TestEventArgs());
            m_SourceModel = new SourceModel(typeof(TestSource), m_EventsContextMock.Object);
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(m_EventFieldName);
        }

        [TearDown]
        public void TearDown()
        {
            m_LoggerMock.Verify();
            m_LoggerScopeMock.Verify();
            m_TypesResolutionServiceMock.Verify();
            m_SourceModelsServiceMock.Verify();
            m_PipelineMock.Verify();
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

            m_TypesResolutionServiceMock
                .Setup(x => x.GetSourceType(m_PipelineEvent.OriginalSender))
                .Returns<object>(x => x.GetType())
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
                .AddEventPipelineConfig(m_PipelineMock.Object);

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
