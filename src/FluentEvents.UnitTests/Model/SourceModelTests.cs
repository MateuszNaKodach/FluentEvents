using System;
using FluentEvents.Model;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelTests
    {
        private Mock<IEventsContext> m_EventsContextMock;
        private SourceModel m_SourceModelWithInvalidArgs;
        private SourceModel m_SourceModelWithInvalidReturnType;

        [SetUp]
        public void SetUp()
        {
            m_EventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);
            m_SourceModelWithInvalidArgs = new SourceModel(typeof(TestSourceWithInvalidArgs), m_EventsContextMock.Object);
            m_SourceModelWithInvalidReturnType = new SourceModel(typeof(TestSourceWithInvalidReturnType), m_EventsContextMock.Object);
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
        }

        private delegate object EventHandlerWithInvalidReturnType<in TEventArgs>(object sender, TEventArgs e);

        private class TestSourceWithInvalidReturnType
        {
            public event EventHandlerWithInvalidReturnType<TestArgs> EventWithArgs;
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
