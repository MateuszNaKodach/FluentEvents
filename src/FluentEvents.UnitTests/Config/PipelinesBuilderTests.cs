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
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IEventSelectionService> m_EventSelectionServiceMock;

        private PipelinesBuilder m_PipelinesBuilder;
        private SourceModel m_SourceModel;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_EventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));

            m_PipelinesBuilder = new PipelinesBuilder(
                m_ServiceProviderMock.Object,
                m_SourceModelsServiceMock.Object,
                m_EventSelectionServiceMock.Object
            );
        }

        [Test]
        public void Event_WithEventName_ShouldCreateSourceModelAndSourceModelEventFieldAndReturnEventConfigurator()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            var eventConfigurator = m_PipelinesBuilder.Event<TestSource, TestEventArgs>(nameof(TestSource.TestEvent));

            AssertValidEventConfigurator(eventConfigurator);
        }

        [Test]
        public void Event_WithEventSelector_ShouldCreateSourceModelAndSourceModelEventFieldAndReturnEventConfigurator()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            m_EventSelectionServiceMock
                .Setup(x => x.GetSelectedEvent(m_SourceModel, action))
                .Returns(new [] { nameof(TestSource.TestEvent) })
                .Verifiable();

            var eventConfigurator = m_PipelinesBuilder.Event<TestSource, TestEventArgs>(action);

            AssertValidEventConfigurator(eventConfigurator);
        }

        private void AssertValidEventConfigurator(EventConfigurator<TestSource, TestEventArgs> eventConfigurator)
        {
            var serviceProvider = eventConfigurator.Get<IServiceProvider>();
            var sourceModel = eventConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventConfigurator.Get<SourceModelEventField>();

            Assert.That(eventConfigurator, Is.Not.Null);
            Assert.That(serviceProvider, Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(m_SourceModel));
            Assert.That(
                sourceModel,
                Has.Property(nameof(SourceModel.EventFields)).With.One.Items.EqualTo(sourceModelEventField)
            );
        }

        [Test]
        public void Event_WithInvalidEventName_ShouldThrow()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, TestEventArgs>("Invalid");
            }, Throws.TypeOf<EventFieldNotFoundException>());
        }

        [Test]
        public void Event_WithMultipleEventsSelected_ShouldThrow()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            m_EventSelectionServiceMock
                .Setup(x => x.GetSelectedEvent(m_SourceModel, action))
                .Returns(new[] { nameof(TestSource.TestEvent), nameof(TestSource.TestEvent2) })
                .Verifiable();

            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, TestEventArgs>(action);
            }, Throws.TypeOf<MoreThanOneEventSelectedException>());
        }

        [Test]
        public void Event_WithNoEventsSelected_ShouldThrow()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            m_EventSelectionServiceMock
                .Setup(x => x.GetSelectedEvent(m_SourceModel, action))
                .Returns(new string[0])
                .Verifiable();

            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, TestEventArgs>(action);
            }, Throws.TypeOf<NoEventsSelectedException>());
        }

        [Test]
        public void Event_WithNullEventName_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, TestEventArgs>((string)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Event_WithNullAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, TestEventArgs>((Action<TestSource, dynamic>)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Event_WithEventNameAndInvalidEventArgsType_ShouldThrow()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, object>(nameof(TestSource.TestEvent));
            }, Throws.TypeOf<EventArgsTypeMismatchException>());
        }

        [Test]
        public void Event_WithEventSelectorAndInvalidEventArgsType_ShouldThrow()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Action<TestSource, dynamic> action = (source, eventHandler) => { };

            m_EventSelectionServiceMock
                .Setup(x => x.GetSelectedEvent(m_SourceModel, action))
                .Returns(new[] { nameof(TestSource.TestEvent) })
                .Verifiable();

            Assert.That(() =>
            {
                m_PipelinesBuilder.Event<TestSource, object>(action);
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
