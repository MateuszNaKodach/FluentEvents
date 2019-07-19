using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Transmission;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Transmission
{
    [TestFixture]
    public class EventReceiversHostedServiceTests
    {
        private Mock<IEventReceiversService> _eventReceiversServiceMock;

        private EventReceiversHostedService _eventReceiversHostedService;

        [SetUp]
        public void SetUp()
        {
            _eventReceiversServiceMock = new Mock<IEventReceiversService>(MockBehavior.Strict);

            _eventReceiversHostedService = new EventReceiversHostedService(_eventReceiversServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _eventReceiversServiceMock.Verify();
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
