using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Transmission;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Transmission
{
    [TestFixture]
    public class EventReceiversHostedServiceTests
    {
        private Mock<IEventsContext> _eventsContextMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IEventReceiversService> _eventReceiversServiceMock;

        private EventReceiversHostedService _eventReceiversHostedService;

        [SetUp]
        public void SetUp()
        {
            _eventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);
            _serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _eventReceiversServiceMock = new Mock<IEventReceiversService>(MockBehavior.Strict);

            _eventsContextMock
                .As<IInfrastructure<IServiceProvider>>()
                .Setup(x => x.Instance)
                .Returns(_serviceProviderMock.Object)
                .Verifiable();

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IEventReceiversService)))
                .Returns(_eventReceiversServiceMock.Object)
                .Verifiable();

            _eventReceiversHostedService = new EventReceiversHostedService(_eventsContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _eventReceiversServiceMock.Verify();
            _eventsContextMock.Verify();
            _serviceProviderMock.Verify();
        }

        [Test]
        public async Task StartAsync_ShouldStartReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _eventReceiversServiceMock
                .Setup(x => x.StartReceiversAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _eventReceiversHostedService.StartAsync(token);
        }

        [Test]
        public async Task StopAsync_ShouldStopReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _eventReceiversServiceMock
                .Setup(x => x.StopReceiversAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _eventReceiversHostedService.StopAsync(token);
        }
    }
}
