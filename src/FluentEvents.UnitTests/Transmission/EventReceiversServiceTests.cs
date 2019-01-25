using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Transmission
{
    [TestFixture]
    public class EventReceiversServiceTests
    {
        private Mock<IEventReceiver> m_EventReceiverMock1;
        private Mock<IEventReceiver> m_EventReceiverMock2;
        private Mock<ILogger<EventReceiversService>> m_LoggerMock;

        private EventReceiversService m_EventReceiversService;

        [SetUp]
        public void SetUp()
        {
            m_EventReceiverMock1 = new Mock<IEventReceiver>(MockBehavior.Strict);
            m_EventReceiverMock2 = new Mock<IEventReceiver>(MockBehavior.Strict);
            m_LoggerMock = new Mock<ILogger<EventReceiversService>>(MockBehavior.Strict);

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();
            
            m_EventReceiversService = new EventReceiversService(
                m_LoggerMock.Object,
                new[]
                {
                    m_EventReceiverMock1.Object,
                    m_EventReceiverMock2.Object,
                }
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_EventReceiverMock1.Verify();
            m_EventReceiverMock2.Verify();
            m_LoggerMock.Verify();
        }

        [Test]
        public async Task StartReceiversAsync_ShouldStartReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStarting,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            m_EventReceiverMock1
                .Setup(x => x.StartReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventReceiverMock2
                .Setup(x => x.StartReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStarted,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await m_EventReceiversService.StartReceiversAsync(token);
        }

        [Test]
        public async Task StopReceiversAsync_ShouldStopReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStopping,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            m_EventReceiverMock1
                .Setup(x => x.StopReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventReceiverMock2
                .Setup(x => x.StopReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStopped,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await m_EventReceiversService.StopReceiversAsync(token);
        }
    }
}
