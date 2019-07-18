using System;
using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Infrastructure
{
    [TestFixture]
    public class AppServiceProviderTests
    {
        private Mock<IServiceProvider> _serviceProviderMock;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProviderMock.Verify();
        }

        [Test]
        public void GetService_ShouldInvokeUnderlyingServiceProvider()
        {
            var service = new object();

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(object)))
                .Returns(service)
                .Verifiable();

            var appServiceProvider = new AppServiceProvider(_serviceProviderMock.Object);

            var returnedService = appServiceProvider.GetService(typeof(object));

            Assert.That(returnedService, Is.EqualTo(service));
        }
    }
}
