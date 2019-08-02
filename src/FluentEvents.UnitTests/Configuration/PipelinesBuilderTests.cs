using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Configuration
{
    [TestFixture]
    public class PipelinesBuilderTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;

        private IPipelinesBuilder _pipelinesBuilder;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);

            _pipelinesBuilder = new PipelinesBuilder(
                _serviceProviderMock.Object
            );
        }
        
        [Test]
        public void Event_ShouldReturnEventConfigurator()
        {
            var eventConfigurator = _pipelinesBuilder.Event<object>();

            var serviceProvider = eventConfigurator.Get<IServiceProvider>();

            Assert.That(eventConfigurator, Is.Not.Null);
            Assert.That(serviceProvider, Is.EqualTo(_serviceProviderMock.Object));
        }
    }
}
