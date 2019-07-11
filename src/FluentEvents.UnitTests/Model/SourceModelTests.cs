using FluentEvents.Model;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelTests
    {
        private SourceModel _sourceModelWithInvalidArgs;
        private SourceModel _sourceModelWithInvalidReturnType;

        [SetUp]
        public void SetUp()
        {
            _sourceModelWithInvalidArgs = new SourceModel(typeof(TestSourceWithInvalidArgs));
            _sourceModelWithInvalidReturnType = new SourceModel(typeof(TestSourceWithInvalidReturnType));
        }

        [Test]
        public void GetOrCreateEventField_WhenModelHasInvalidEventHandlerArgs_ShouldThrow()
        {
            Assert.That(
                () =>
                {
                    _sourceModelWithInvalidArgs.GetOrCreateEventField(
                        nameof(TestSourceWithInvalidArgs.EventWithArgs)
                    );
                },
                Throws.TypeOf<InvalidEventHandlerParametersException>());
        }

        [Test]
        public void GetOrCreateEventField_WhenModelHasInvalidEventHandlerReturnType_ShouldThrow()
        {
            Assert.That(
                () =>
                {
                    _sourceModelWithInvalidReturnType.GetOrCreateEventField(
                        nameof(TestSourceWithInvalidReturnType.EventWithArgs)
                    );
                }, Throws.TypeOf<InvalidEventHandlerReturnTypeException>());
        }

        private delegate void EventHandlerWithInvalidArgs<in TEventArgs>(object sender, TEventArgs e, object invalidArg);

        private class TestSourceWithInvalidArgs
        {
            public event EventHandlerWithInvalidArgs<TestEventArgs> EventWithArgs;
        }

        private delegate object EventHandlerWithInvalidReturnType<in TEventArgs>(object sender, TEventArgs e);

        private class TestSourceWithInvalidReturnType
        {
            public event EventHandlerWithInvalidReturnType<TestEventArgs> EventWithArgs;
        }

        private class TestEventArgs
        {
            public int Prop { get; set; }
        }
    }
}
