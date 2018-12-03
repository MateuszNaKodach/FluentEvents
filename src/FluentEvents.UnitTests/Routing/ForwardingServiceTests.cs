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
        private SourceModel m_SourceModel;
        private EventsScope m_EventsScope;
        private Mock<IRoutingService> m_EventsRoutingServiceMock;
        private EventsContextImpl m_EventsContext;
        private ForwardingService m_ForwardingService;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_SourceModel = new SourceModel(typeof(TestSource), m_EventsContext);
            m_EventsScope = new EventsScope();
            m_EventsRoutingServiceMock = new Mock<IRoutingService>(MockBehavior.Strict);
            m_ForwardingService = new ForwardingService(m_EventsRoutingServiceMock.Object);
        }

        [Test]
        public async Task ForwardEventsToRouting_ShouldAddEventHandlers()
        {
            var source = new TestSource();
            m_EventsRoutingServiceMock
                .Setup(x => x.RouteEventAsync(It.IsAny<PipelineEvent>(), m_EventsScope))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_SourceModel.GetOrCreateEventField(nameof(TestSource.NoArgsEvent));
            m_SourceModel.GetOrCreateEventField(nameof(TestSource.EventWithArgs));
            m_SourceModel.GetOrCreateEventField(nameof(TestSource.AsyncNoArgsEvent));
            m_SourceModel.GetOrCreateEventField(nameof(TestSource.AsyncEventWithArgs));

            m_ForwardingService.ForwardEventsToRouting(
                m_SourceModel,
                source,
                m_EventsScope
            );

            await source.RaiseEvents();

            Assert.That(m_EventsRoutingServiceMock.Invocations, Has.Exactly(4).Items);
        }

        [Test]
        public void ForwardEventsToRouting_WithSourceNotMatchingModelType_ShouldThrow()
        {
            var source = new object();

            Assert.That(() =>
            {
                m_ForwardingService.ForwardEventsToRouting(
                    m_SourceModel,
                    source,
                    m_EventsScope
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
