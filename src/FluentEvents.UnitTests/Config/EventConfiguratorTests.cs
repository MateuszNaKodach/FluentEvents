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
        private Mock<IServiceProvider> _serviceProviderMock;
        private SourceModel _sourceModel;
        private SourceModelEventField _sourceModelEventField;

        private EventConfigurator<TestSource, TestEventArgs> _eventConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(TestSource));
            _sourceModelEventField = _sourceModel.GetOrCreateEventField(nameof(TestSource.TestEvent));

            _eventConfigurator = new EventConfigurator<TestSource, TestEventArgs>(
                _serviceProviderMock.Object,
                _sourceModel,
                _sourceModelEventField
            );
        }

        [Test]
        public void IsWatched_ShouldAddPipelineAndReturnEventPipelineConfigurator()
        {
            var eventPipelineConfigurator = _eventConfigurator.IsWatched();
            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            var sourceModel = eventPipelineConfigurator.Get<SourceModel>();
            var sourceModelEventField = eventPipelineConfigurator.Get<SourceModelEventField>();

            Assert.That(eventPipelineConfigurator, Is.Not.Null);
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
            Assert.That(sourceModel, Is.EqualTo(_sourceModel));
            Assert.That(sourceModelEventField, Is.EqualTo(_sourceModelEventField));
            Assert.That(pipeline, Is.Not.Null);
            Assert.That(
                _sourceModelEventField,
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
