using System;
using System.Threading.Tasks;
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
        private Mock<ILogger<RoutingService>> _loggerMock;
        private Mock<IDisposable> _loggerScopeMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IPipeline> _pipelineMock;

        private EventsScope _eventsScope;
        private RoutingService _routingService;
        private PipelineEvent _pipelineEvent;
        private SourceModel _sourceModel1;
        private SourceModel _sourceModel2;

        private readonly string _eventFieldName = nameof(TestSource2.TestEvent);

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RoutingService>>(MockBehavior.Strict);
            _loggerScopeMock = new Mock<IDisposable>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _pipelineMock = new Mock<IPipeline>(MockBehavior.Strict);

            _eventsScope = new EventsScope();
            _routingService = new RoutingService(
                _loggerMock.Object,
                _sourceModelsServiceMock.Object
            );
            _pipelineEvent = new PipelineEvent(
                typeof(TestSource3),
                _eventFieldName,
                new TestSource3(),
                new TestEventArgs()
            );
            _sourceModel2 = new SourceModel(typeof(TestSource2));
            _sourceModel2.GetOrCreateEventField(_eventFieldName);

            _sourceModel1 = new SourceModel(typeof(TestSource1));
            _sourceModel1.GetOrCreateEventField(_eventFieldName).AddPipeline(_pipelineMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _loggerScopeMock.Verify();
            _sourceModelsServiceMock.Verify();
            _pipelineMock.Verify();
        }

        [Test]
        public async Task RouteEventAsync_ShouldProcessEvent()
        {
            _pipelineMock
                .Setup(x => x.ProcessEventAsync(_pipelineEvent, _eventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(TestSource1)))
                .Returns(_sourceModel1)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(TestSource2)))
                .Returns(_sourceModel2)
                .Verifiable();

            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(TestSource3)))
                .Returns<SourceModel>(null)
                .Verifiable();

            _loggerScopeMock
                .Setup(x => x.Dispose())
                .Verifiable();

            _loggerMock
                .Setup(x => x.BeginScope(It.IsAny<object>()))
                .Returns(_loggerScopeMock.Object)
                .Verifiable();

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    RoutingLoggerMessages.EventIds.EventRoutedToPipeline,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await _routingService.RouteEventAsync(_pipelineEvent, _eventsScope);
        }

        

        private class TestSource1
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestSource2 : TestSource1
        {
        }

        private class TestSource3 : TestSource2
        {
        }

        private class TestEventArgs
        {
        }
    }
}
