using System;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Common
{
    [TestFixture]
    public class EventSenderBaseTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<IQueueClient> _queueClientMock;

        private EventSenderBase _eventSenderBase;
        private PipelineEvent _pipelineEvent;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _queueClientMock = new Mock<IQueueClient>(MockBehavior.Strict);

            _eventSenderBase = new EventSenderImpl(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                _queueClientMock.Object
            );

            _pipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _queueClientMock.Verify();
        }

        [Test]
        public async Task SendAsync_ShouldSendMessage()
        {
            var serializedEventBytes = new byte[] {1, 2, 3, 4, 5};

            _eventsSerializationServiceMock
                .Setup(x => x.SerializeEvent(_pipelineEvent))
                .Returns(serializedEventBytes)
                .Verifiable();

            _queueClientMock
                .Setup(x => x.SendAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    LoggerMessages.EventIds.MessageSent,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await _eventSenderBase.SendAsync(_pipelineEvent);
        }

        [Test]
        public void Dispose_ShouldCloseTopicClient()
        {
            _queueClientMock
                .Setup(x => x.CloseAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _eventSenderBase.Dispose();
        }
        private class EventSenderImpl : EventSenderBase
        {
            public EventSenderImpl(
                ILogger logger,
                IEventsSerializationService eventsSerializationService,
                IQueueClient queueClient
            )
                : base(logger, eventsSerializationService, queueClient)
            {
            }
        }
    }
}
