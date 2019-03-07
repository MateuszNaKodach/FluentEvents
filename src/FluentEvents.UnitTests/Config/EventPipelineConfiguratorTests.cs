using System;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class EventPipelineConfiguratorTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;
        private Pipeline m_Pipeline;
        private EventConfigurator<TestSource, TestEventArgs> m_EventConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));
            m_Pipeline = new Pipeline(m_ServiceProviderMock.Object);
            m_EventConfigurator = new EventConfigurator<TestSource, TestEventArgs>(
                m_ServiceProviderMock.Object,
                m_SourceModel,
                m_SourceModelEventField
            );
        }

        [Test]
        public void Ctor_WithEventConfigurator_ShouldGetPropertiesFromEventConfigurator()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                m_Pipeline,
                m_EventConfigurator
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(pipeline, Is.EqualTo(m_Pipeline));
            Assert.That(serviceProvider, Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(m_SourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(m_SourceModelEventField));
        }

        [Test]
        public void Ctor_WithSingleParameters_ShouldCreateCustomInstance()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                m_SourceModel,
                m_SourceModelEventField,
                m_ServiceProviderMock.Object,
                m_Pipeline
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(pipeline, Is.EqualTo(m_Pipeline));
            Assert.That(serviceProvider, Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(m_SourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(m_SourceModelEventField));
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
