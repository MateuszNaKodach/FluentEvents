using System;
using FluentEvents.Model;
using FluentEvents.Routing;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelTests
    {
        private EventsContext m_EventsContext;
        private SourceModel m_SourceModelWithInvalidArgs;
        private SourceModel m_SourceModelWithInvalidReturnType;
        private SourceModel m_SourceModel;
        private EventsScope m_EventsScope;
        private Mock<IRoutingService> m_EventsRoutingServiceMock;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_SourceModel = new SourceModel(typeof(TestSource), m_EventsContext);
            m_SourceModelWithInvalidArgs = new SourceModel(typeof(TestSourceWithInvalidArgs), m_EventsContext);
            m_SourceModelWithInvalidReturnType = new SourceModel(typeof(TestSourceWithInvalidReturnType), m_EventsContext);
            m_EventsScope = new EventsScope();
            m_EventsRoutingServiceMock = new Mock<IRoutingService>(MockBehavior.Strict);
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
            public event EventHandler<TestArgs> EventWithArgs;
        }

        private class TestArgs
        {
            public int Prop { get; set; }
        }
    }
}
