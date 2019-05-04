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
        private Mock<IServiceProvider> _serviceProviderMock;
        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;
        private Pipeline _pipeline;
        private EventConfigurator<TestSource, TestEventArgs> _eventConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestSource));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));
            _pipeline = new Pipeline(_serviceProviderMock.Object);
            _eventConfigurator = new EventConfigurator<TestSource, TestEventArgs>(
                _serviceProviderMock.Object,
                _sourceModel,
                _sourceModelEventField
            );
        }

        [Test]
        public void Ctor_WithEventConfigurator_ShouldInheritPropertyValuesFromEventConfigurator()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                _pipeline,
                _eventConfigurator
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(pipeline, Is.EqualTo(_pipeline));
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(_sourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(_sourceModelEventField));
        }

        [Test]
        public void Ctor_WithAllParameters_ShouldSetPropertiesFromParameters()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<TestSource, TestEventArgs>(
                _sourceModel,
                _sourceModelEventField,
                _serviceProviderMock.Object,
                _pipeline
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(pipeline, Is.EqualTo(_pipeline));
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(_sourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(_sourceModelEventField));
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
