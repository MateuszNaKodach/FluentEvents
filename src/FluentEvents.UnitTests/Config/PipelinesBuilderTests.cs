using System;
using FluentEvents.Infrastructure;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Utils;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class PipelinesBuilderTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IEventSelectionService> _eventSelectionServiceMock;

        private PipelinesBuilder _pipelinesBuilder;
        private SourceModel _sourceModel;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _eventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestSource));

            _pipelinesBuilder = new PipelinesBuilder(
                _serviceProviderMock.Object,
                _sourceModelsServiceMock.Object,
                _eventSelectionServiceMock.Object
            );
        }

        [Test]
        public void EventWithEventNameShouldCreateSourceModelAndSourceModelEventFieldAndReturnEventConfigurator()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(_sourceModel)
                .Verifiable();

            var eventConfigurator = _pipelinesBuilder.Event<TestSource, TestEventArgs>(nameof(TestSource.TestEvent));

            AssertValidEventConfigurator(eventConfigurator);
        }

        [Test]
        public void Event_WithEventSelector_ShouldCreateSourceModelWithEventFieldAndReturnEventConfigurator()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(_sourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            _eventSelectionServiceMock
                .Setup(x => x.GetSingleSelectedEvent(_sourceModel, action))
                .Returns(nameof(TestSource.TestEvent))
                .Verifiable();

            var eventConfigurator = _pipelinesBuilder.Event<TestSource, TestEventArgs>(action);

            AssertValidEventConfigurator(eventConfigurator);
        }

        private void AssertValidEventConfigurator(EventConfigurator<TestSource, TestEventArgs> eventConfigurator)
        {
            var serviceProvider = eventConfigurator.Get<IServiceProvider>();
            var sourceModel = eventConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventConfigurator.Get<SourceModelEventField>();

            Assert.That(eventConfigurator, Is.Not.Null);
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(_sourceModel));
            Assert.That(
                sourceModel,
                Has.Property(nameof(SourceModel.EventFields)).With.One.Items.EqualTo(sourceModelEventField)
            );
        }

        [Test]
        public void Event_WithInvalidEventName_ShouldThrow()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(_sourceModel)
                .Verifiable();

            Assert.That(() =>
            {
                _pipelinesBuilder.Event<TestSource, TestEventArgs>("Invalid");
            }, Throws.TypeOf<EventFieldNotFoundException>());
        }

        [Test]
        public void Event_WithNullEventName_ShouldThrow()
        {
            Assert.That(() =>
            {
                _pipelinesBuilder.Event<TestSource, TestEventArgs>((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Event_WithNullAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _pipelinesBuilder.Event<TestSource, TestEventArgs>((Action<TestSource, dynamic>)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Event_WithEventNameAndInvalidEventArgsType_ShouldThrow()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(_sourceModel)
                .Verifiable();

            Assert.That(() =>
            {
                _pipelinesBuilder.Event<TestSource, object>(nameof(TestSource.TestEvent));
            }, Throws.TypeOf<EventArgsTypeMismatchException>());
        }

        [Test]
        public void Event_WithEventSelectorAndInvalidEventArgsType_ShouldThrow()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(_sourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            _eventSelectionServiceMock
                .Setup(x => x.GetSingleSelectedEvent(_sourceModel, action))
                .Returns(nameof(TestSource.TestEvent))
                .Verifiable();

            Assert.That(() =>
            {
                _pipelinesBuilder.Event<TestSource, object>(action);
            }, Throws.TypeOf<EventArgsTypeMismatchException>());
        }

        private class TestSource
        {
            public event EventHandler<TestEventArgs> TestEvent;
            public event EventHandler<TestEventArgs> TestEvent2;
        }

        private class TestEventArgs
        {

        }
    }
}
