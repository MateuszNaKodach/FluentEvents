using System;
using FluentEvents.Infrastructure;
using FluentEvents.Config;
using FluentEvents.Model;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class PipelinesBuilderTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;

        private PipelinesBuilder m_PipelinesBuilder;
        private SourceModel m_SourceModel;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));

            m_PipelinesBuilder = new PipelinesBuilder(m_ServiceProviderMock.Object, m_SourceModelsServiceMock.Object);
        }

        [Test]
        public void Event_ShouldCreateSourceModelAndSourceModelEventFieldAndReturnEventConfigurator()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetOrCreateSourceModel(typeof(TestSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            var eventConfigurator = m_PipelinesBuilder.Event<TestSource, TestEventArgs>(nameof(TestSource.TestEvent));
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
        public void Event_WithInvalidEventArgsType_ShouldThrow()
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

        private class TestSource
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        private class TestEventArgs
        {

        }
    }
}
