using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Common
{
    [TestFixture]
    public class EventReceiverBaseTests
    {
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";

        private EventReceiverConfigImpl _config;

        private Mock<ILogger> _loggerMock;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;

        private EventReceiverImpl _eventReceiver;
        private Func<Message, CancellationToken, Task> _messageHandler;
        private MessageHandlerOptions _messageHandlerOptions;
        private Mock<IReceiverClient> _receiverClientMock;

        [SetUp]
        public async Task SetUp()
        {
            _config = new EventReceiverConfigImpl
            {
                ReceiveConnectionString = ReceiveConnectionString,
                MaxConcurrentMessages = 10
            };
            _loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _receiverClientMock = new Mock<IReceiverClient>(MockBehavior.Strict);

            _eventReceiver = new EventReceiverImpl(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                _publishingServiceMock.Object,
                _config,
                _receiverClientMock.Object
            );

            var cts = new CancellationTokenSource();

            _messageHandler = null;
            _messageHandlerOptions = null;

            _receiverClientMock
                .Setup(x => x.RegisterMessageHandler(
                    It.IsAny<Func<Message, CancellationToken, Task>>(),
                    It.Is<MessageHandlerOptions>(y => y.MaxConcurrentCalls == _config.MaxConcurrentMessages)
                ))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((x, y) =>
                {
                    _messageHandler = x;
                    _messageHandlerOptions = y;
                })
                .Verifiable();

            await _eventReceiver.StartReceivingAsync(cts.Token);
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _publishingServiceMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _receiverClientMock.Verify();
        }

        [Test]
        public void MessageHandler_ShouldDeserializeAndPublishEventToGlobalSubscriptions()
        {
            var messageBytes = new byte[] { 1, 2, 3, 4, 5 };
            var pipelineEvent = new PipelineEvent(
                typeof(object),
                "",
                new object(),
                new object()
            );

            _eventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Returns(pipelineEvent)
                .Verifiable();

            _publishingServiceMock
                .Setup(x => x.PublishEventToGlobalSubscriptionsAsync(pipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _receiverClientMock
                .Setup(x => x.CompleteAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var message = CreateReceivedMessage(messageBytes);

            _messageHandler(message, CancellationToken.None);
        }

        [Test]
        public void MessageHandler_ShouldLogExceptions()
        {
            var messageBytes = new byte[] {1, 2, 3};

            var exception = new Exception();

            _eventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Throws(exception)
                .Verifiable();

            _receiverClientMock
                .Setup(x => x.AbandonAsync(It.IsAny<string>(), null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    LoggerMessages.EventIds.MessagesProcessingThrew,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            var message = CreateReceivedMessage(messageBytes);

            _messageHandler(message, CancellationToken.None);
        }

        [Test]
        public void ExceptionReceivedHandler_ShouldLogException()
        {
            var exceptionReceivedEventArgs = new ExceptionReceivedEventArgs(new Exception(), "", "", "", "");

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    LoggerMessages.EventIds.ServiceBusExceptionReceived,
                    It.IsAny<object>(),
                    exceptionReceivedEventArgs.Exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            _messageHandlerOptions.ExceptionReceivedHandler(exceptionReceivedEventArgs);
        }

        private static Message CreateReceivedMessage(byte[] messageBytes)
        {
            var message = new Message(messageBytes);
            typeof(Message.SystemPropertiesCollection)
                .GetProperty(nameof(Message.SystemProperties.SequenceNumber))
                .SetValue(message.SystemProperties, 1);

            return message;
        }
    }

    internal class EventReceiverConfigImpl : EventReceiverConfigBase { }

    internal class EventReceiverImpl : EventReceiverBase
    {
        private readonly IReceiverClient _receiverClient;

        public EventReceiverImpl(
            ILogger logger,
            IEventsSerializationService eventsSerializationService,
            IPublishingService publishingService,
            EventReceiverConfigBase config,
            IReceiverClient receiverClient
        ) : base(logger, eventsSerializationService, publishingService, config)
        {
            _receiverClient = receiverClient;
        }

        protected internal override Task<IReceiverClient> CreateReceiverClientAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_receiverClient);
        }
    }
}
