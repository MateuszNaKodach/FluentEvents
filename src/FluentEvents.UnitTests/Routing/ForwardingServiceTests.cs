using System;
using System.Threading.Tasks;
using AsyncEvent;
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

        private SourceModel _sourceModel;
        private EventsScope _eventsScope;
        private ForwardingService _forwardingService;

        [SetUp]
        public void SetUp()
        {
            _routingServiceMock = new Mock<IRoutingService>(MockBehavior.Strict);

            _sourceModel = new SourceModel(typeof(TestSource));
            _eventsScope = new EventsScope();
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
                .Setup(x => x.RouteEventAsync(It.IsAny<PipelineEvent>(), _eventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _sourceModel.GetOrCreateEventField(nameof(TestSource.NoArgsEvent));
            _sourceModel.GetOrCreateEventField(nameof(TestSource.EventWithArgs));
            _sourceModel.GetOrCreateEventField(nameof(TestSource.AsyncNoArgsEvent));
            _sourceModel.GetOrCreateEventField(nameof(TestSource.AsyncEventWithArgs));

            _forwardingService.ForwardEventsToRouting(
                _sourceModel,
                source,
                _eventsScope
            );

            await source.RaiseEvents();

            Assert.That(_routingServiceMock.Invocations, Has.Exactly(4).Items);
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
                    _eventsScope
                );
            }, Throws.TypeOf<SourceDoesNotMatchModelTypeException>());
        }

        private class TestSource
        {
            public event EventHandler NoArgsEvent;
            public event EventHandler<TestArgs> EventWithArgs;
            public event AsyncEventHandler AsyncNoArgsEvent;
            public event AsyncEventHandler<TestArgs> AsyncEventWithArgs;

            public async Task RaiseEvents()
            {
                NoArgsEvent?.Invoke(this, EventArgs.Empty);
                EventWithArgs?.Invoke(this, new TestArgs());
                await (AsyncNoArgsEvent?.InvokeAsync(this, EventArgs.Empty) ?? Task.CompletedTask);
                await (AsyncEventWithArgs?.InvokeAsync(this, new TestArgs()) ?? Task.CompletedTask);
            }
        }

        private class TestArgs
        {
            public int Prop { get; set; }
        }
    }
}
