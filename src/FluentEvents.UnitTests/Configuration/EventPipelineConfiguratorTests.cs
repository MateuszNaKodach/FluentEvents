using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Configuration
{
    [TestFixture]
    public class EventPipelineConfiguratorTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;

        private Pipeline _pipeline;
        private EventConfiguration<object> _eventConfiguration;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipeline = new Pipeline(_serviceProviderMock.Object);
            _eventConfiguration = new EventConfiguration<object>(
                _serviceProviderMock.Object
            );
        }

        [Test]
        public void Ctor_WithEventConfigurator_ShouldInheritPropertyValuesFromEventConfigurator()
        {
            var eventPipelineConfigurator = new EventPipelineConfiguration<object>(
                _pipeline,
                _eventConfiguration
            );

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();

            Assert.That(pipeline, Is.EqualTo(_pipeline));
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
        }

        [Test]
        public void Ctor_WithAllParameters_ShouldSetPropertiesFromParameters()
        {
            var eventPipelineConfigurator = new EventPipelineConfiguration<object>(
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
