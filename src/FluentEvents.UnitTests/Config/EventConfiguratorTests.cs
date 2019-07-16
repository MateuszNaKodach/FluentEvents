using System;
using FluentEvents.Configuration;
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
        private Mock<IPipelinesService> _pipelinesServiceMock;

        private EventConfigurator<object> _eventConfigurator;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _pipelinesServiceMock = new Mock<IPipelinesService>(MockBehavior.Strict);

            _eventConfigurator = new EventConfigurator<object>(
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

            var eventPipelineConfigurator = _eventConfigurator.IsPiped();

            Assert.That(eventPipelineConfigurator, Is.Not.Null);

            var serviceProvider = eventPipelineConfigurator.Get<IServiceProvider>();
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));

            var pipeline = eventPipelineConfigurator.Get<IPipeline>();
            Assert.That(pipeline, Is.Not.Null);
        }
    }
}
