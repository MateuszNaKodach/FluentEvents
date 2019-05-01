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
        private Mock<IEventReceiver> _eventReceiverMock1;
        private Mock<IEventReceiver> _eventReceiverMock2;
        private Mock<ILogger<EventReceiversService>> _loggerMock;

        private EventReceiversService _eventReceiversService;

        [SetUp]
        public void SetUp()
        {
            _eventReceiverMock1 = new Mock<IEventReceiver>(MockBehavior.Strict);
            _eventReceiverMock2 = new Mock<IEventReceiver>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<EventReceiversService>>(MockBehavior.Strict);

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();
            
            _eventReceiversService = new EventReceiversService(
                _loggerMock.Object,
                new[]
                {
                    _eventReceiverMock1.Object,
                    _eventReceiverMock2.Object,
                }
            );
        }

        [TearDown]
        public void TearDown()
        {
            _eventReceiverMock1.Verify();
            _eventReceiverMock2.Verify();
            _loggerMock.Verify();
        }

        [Test]
        public async Task StartReceiversAsync_ShouldStartReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStarting,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            _eventReceiverMock1
                .Setup(x => x.StartReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _eventReceiverMock2
                .Setup(x => x.StartReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStarted,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await _eventReceiversService.StartReceiversAsync(token);
        }

        [Test]
        public async Task StopReceiversAsync_ShouldStopReceivers()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStopping,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            _eventReceiverMock1
                .Setup(x => x.StopReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _eventReceiverMock2
                .Setup(x => x.StopReceivingAsync(token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    TransmissionLoggerMessages.EventIds.EventReceiverStopped,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await _eventReceiversService.StopReceiversAsync(token);
        }
    }
}
