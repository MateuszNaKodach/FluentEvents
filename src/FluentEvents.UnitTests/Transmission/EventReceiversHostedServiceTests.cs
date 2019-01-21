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
        private Mock<IEventReceiversService> m_EventReceiversServiceMock;

        private EventReceiversHostedService m_EventReceiversHostedService;

        [SetUp]
        public void SetUp()
        {
            m_EventReceiversServiceMock = new Mock<IEventReceiversService>(MockBehavior.Strict);

            m_EventReceiversHostedService = new EventReceiversHostedService(m_EventReceiversServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            m_EventReceiversServiceMock.Verify();
        }

        [Test]
        public async Task StartAsync_ShouldStartReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            m_EventReceiversServiceMock
                .Setup(x => x.StartReceiversAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_EventReceiversHostedService.StartAsync(token);
        }

        [Test]
        public async Task StopAsync_ShouldStopReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            m_EventReceiversServiceMock
                .Setup(x => x.StopReceiversAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_EventReceiversHostedService.StopAsync(token);
        }
    }
}
