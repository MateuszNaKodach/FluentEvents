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
    public class EventConfiguratorTests
    {
        private Mock<IServiceProvider> m_ServiceProviderMock;
        private SourceModel m_SourceModel;
        private SourceModelEventField m_SourceModelEventField;

        private EventConfigurator<TestSource, TestEventArgs> m_EventConfigurator;

        [SetUp]
        public void SetUp()
        {
            m_ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(TestSource));
            m_SourceModelEventField = m_SourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));

            m_EventConfigurator = new EventConfigurator<TestSource, TestEventArgs>(
                m_ServiceProviderMock.Object,
                m_SourceModel,
                m_SourceModelEventField
            );
        }

        [Test]
        public void IsForwardedToPipeline_ShouldAddPipelineAndReturnEventPipelineConfigurator()
        {
            var eventPipelineConfigurator = m_EventConfigurator.IsForwardedToPipeline();
            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(eventPipelineConfigurator, Is.Not.Null);
            Assert.That(serviceProvider, Is.EqualTo(m_ServiceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(m_SourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(m_SourceModelEventField));
            Assert.That(pipeline, Is.Not.Null);
            Assert.That(
                m_SourceModelEventField,
                Has.Property(nameof(SourceModelEventField.Pipelines)).With.One.Items.EqualTo(pipeline)
            );
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
