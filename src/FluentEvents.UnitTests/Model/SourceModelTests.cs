using FluentEvents.Model;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelTests
    {
        private SourceModel _sourceModelWithInvalidArgs;
        private SourceModel _sourceModelWithInvalidReturnType;
        private SourceModel _sourceModeWithValidEvents;

        [SetUp]
        public void SetUp()
        {
            _sourceModelWithInvalidArgs = new SourceModel(typeof(TestSourceWithInvalidArgs));
            _sourceModelWithInvalidReturnType = new SourceModel(typeof(TestSourceWithInvalidReturnType));
            _sourceModeWithValidEvents = new SourceModel(typeof(TestSourceWithValidEvents));
        }

        [Test]
        public void EventFields_WhenModelHasOnlyEventHandlersWithInvalidArgs_ShouldBeEmpty()
        {
            Assert.That(_sourceModelWithInvalidArgs.EventFields, Is.Empty);
        }

        [Test]
        public void EventFields_WhenModelHasOnlyEventHandlersWithInvalidReturnTypes_ShouldBeEmpty()
        {
            Assert.That(_sourceModelWithInvalidReturnType.EventFields, Is.Empty);
        }

        [Test]
        public void EventFields_WhenModelHasValidEventHandler_ShouldHaveEvent()
        {
            Assert.That(_sourceModeWithValidEvents.EventFields, Has.One.Items);
        }

        private delegate void EventHandlerWithInvalidArgs<in TEvent>(TEvent e, object extraArg);

        private class TestSourceWithInvalidArgs
        {
#pragma warning disable 67
            public event EventHandlerWithInvalidArgs<object> EventWithArgs;
#pragma warning restore 67
        }

        private delegate object EventHandlerWithInvalidReturnType<in TEvent>(TEvent e);

        private class TestSourceWithInvalidReturnType
        {
#pragma warning disable 67
            public event EventHandlerWithInvalidReturnType<object> EventWithArgs;
#pragma warning restore 67
        }

        private class TestSourceWithValidEvents
        {
#pragma warning disable 67
            public event DomainEventHandler<object> ValidEvent;
#pragma warning restore 67
        }
    }
}
