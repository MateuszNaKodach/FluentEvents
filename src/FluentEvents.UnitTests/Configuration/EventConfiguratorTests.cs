using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Configuration
{
    [TestFixture]
    public class EventConfiguratorTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IPipelinesService> _pipelinesServiceMock;

        private EventConfiguration<object> _eventConfiguration;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipelinesServiceMock = new Mock<IPipelinesService>(MockBehavior.Strict);

            _eventConfiguration = new EventConfiguration<object>(
                _serviceProviderMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
            _pipelinesServiceMock.Verify();
        }

        [Test]
        public void IsPiped_ShouldAddPipelineAndReturnEventPipelineConfigurator()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IPipelinesService)))
                .Returns(_pipelinesServiceMock.Object)
                .Verifiable();

            _pipelinesServiceMock
                .Setup(x => x.AddPipeline(typeof(object), It.IsAny<IPipeline>()))
                .Verifiable();

            var eventPipelineConfigurator = _eventConfiguration.IsPiped();

            Assert.That(eventPipelineConfigurator, Is.Not.Null);

            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            Assert.That(pipeline, Is.Not.Null);
        }
    }
}
