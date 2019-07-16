using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Routing;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Routing
{
    [TestFixture]
    public class ForwardingServiceTests
    {
        private Mock<IRoutingService> _routingServiceMock;
        private Mock<IEventsScope> _eventsScope;

        private SourceModel _sourceModel;
        private ForwardingService _forwardingService;

        [SetUp]
        public void SetUp()
        {
            _routingServiceMock = new Mock<IRoutingService>(MockBehavior.Strict);
            _eventsScope = new Mock<IEventsScope>(MockBehavior.Strict);

            _sourceModel = new SourceModel(typeof(TestSource));
            _forwardingService = new ForwardingService(_routingServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _routingServiceMock.Verify();
        }

        [Test]
        public async Task ForwardEventsToRouting_ShouldAddEventHandlers()
        {
            var source = new TestSource();
            _routingServiceMock
                .Setup(x => x.RouteEventAsync(It.IsAny<PipelineEvent>(), _eventsScope.Object))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _forwardingService.ForwardEventsToRouting(
                _sourceModel,
                source,
                _eventsScope.Object
            );

            await source.RaiseEvents();

            Assert.That(_routingServiceMock.Invocations, Has.Exactly(2).Items);
        }

        [Test]
        public void ForwardEventsToRouting_WithSourceNotMatchingModelType_ShouldThrow()
        {
            var source = new object();

            Assert.That(() =>
            {
                _forwardingService.ForwardEventsToRouting(
                    _sourceModel,
                    source,
                    _eventsScope.Object
                );
            }, Throws.TypeOf<SourceDoesNotMatchModelTypeException>());
        }

        private class TestSource
        {
            public event DomainEventHandler<TestEvent> Event;
            public event AsyncDomainEventHandler<TestEvent> AsyncEvent;

            public async Task RaiseEvents()
            {
                Event?.Invoke(new TestEvent());
                await (AsyncEvent?.Invoke(new TestEvent()) ?? Task.CompletedTask);
            }
        }

        private class TestEvent
        {
            public int Prop { get; set; }
        }
    }
}
