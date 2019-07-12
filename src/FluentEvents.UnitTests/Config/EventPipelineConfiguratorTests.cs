using System;
using FluentEvents.Config;
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

        private Pipeline _pipeline;
        private EventConfigurator<object> _eventConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipeline = new Pipeline(_serviceProviderMock.Object);
            _eventConfigurator = new EventConfigurator<object>(
                _serviceProviderMock.Object
            );
        }

        [Test]
        public void Ctor_WithEventConfigurator_ShouldInheritPropertyValuesFromEventConfigurator()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<object>(
                _pipeline,
                _eventConfigurator
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();

            Assert.That(pipeline, Is.EqualTo(_pipeline));
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
        }

        [Test]
        public void Ctor_WithAllParameters_ShouldSetPropertiesFromParameters()
        {
            var eventPipelineConfigurator = new EventPipelineConfigurator<object>(
                _serviceProviderMock.Object,
                _pipeline
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();

            Assert.That(pipeline, Is.EqualTo(_pipeline));
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
        }
    }
}
