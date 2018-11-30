using System;
using System.Threading.Tasks;
using AsyncEvent;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Routing;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelTests
    {
        private EventsContext m_EventsContext;
        private SourceModel m_SourceModel;
        private SourceModel m_SourceModelWithInvalidArgs;
        private SourceModel m_SourceModelWithInvalidReturnType;
        private EventsScope m_EventsScope;
        private Mock<IEventsRoutingService> m_EventsRoutingServiceMock;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_SourceModel = new SourceModel(typeof(TestSource), m_EventsContext);
            m_SourceModelWithInvalidArgs = new SourceModel(typeof(TestSourceWithInvalidArgs), m_EventsContext);
            m_SourceModelWithInvalidReturnType = new SourceModel(typeof(TestSourceWithInvalidReturnType), m_EventsContext);
            m_EventsScope = new EventsScope();
            m_EventsRoutingServiceMock = new Mock<IEventsRoutingService>(MockBehavior.Strict);
        }

        [Test]
        public void GetOrCreateEventField_WhenModelHasInvalidEventHandlerArgs_ShouldThrow()
        {
            Assert.That(
                () =>
                {
                    m_SourceModelWithInvalidArgs.GetOrCreateEventField(
                        nameof(TestSourceWithInvalidArgs.EventWithArgs)
                    );
                },
                Throws.TypeOf<InvalidEventHandlerArgsException>());
        }

        [Test]
        public void GetOrCreateEventField_WhenModelHasInvalidEventHandlerReturnType_ShouldThrow()
        {
            Assert.That(
                () =>
                {
                    m_SourceModelWithInvalidReturnType.GetOrCreateEventField(
                        nameof(TestSourceWithInvalidReturnType.EventWithArgs)
                    );
                }, Throws.TypeOf<InvalidEventHandlerReturnTypeException>());
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

            m_SourceModel.ForwardEventsToRouting(
                source,
                m_EventsRoutingServiceMock.Object, 
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
                m_SourceModel.ForwardEventsToRouting(
                    source,
                    m_EventsRoutingServiceMock.Object,
                    m_EventsScope
                );
            }, Throws.TypeOf<SourceDoesNotMatchModelTypeException>());
        }

        private delegate void EventHandlerWithInvalidArgs<in TEventArgs>(object sender, TEventArgs e, object invalidArg);

        private class TestSourceWithInvalidArgs
        {
            public event EventHandlerWithInvalidArgs<TestArgs> EventWithArgs;

            public void RaiseEvents()
            {
                EventWithArgs?.Invoke(this, new TestArgs(), null);
            }
        }

        private delegate object EventHandlerWithInvalidReturnType<in TEventArgs>(object sender, TEventArgs e);

        private class TestSourceWithInvalidReturnType
        {
            public event EventHandlerWithInvalidReturnType<TestArgs> EventWithArgs;

            public void RaiseEvents()
            {
                EventWithArgs?.Invoke(this, new TestArgs());
            }
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
