using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Topics.Receiving;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class AzureTopicEventReceiverTests
    {
        private const string TopicPath = "TopicPath";
        private const string ManagementConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=management;SharedAccessKey=0;EntityPath=0";
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";
        private const string SubscriptionName = "SubscriptionName";

        private AzureServiceBusTopicEventReceiverConfig _config;

        private Mock<ILogger<AzureServiceBusTopicEventReceiver>> _loggerMock;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<ITopicSubscriptionsService> _topicSubscriptionsServiceMock;
        private Mock<ISubscriptionClientFactory> _subscriptionClientFactoryMock;
        private Mock<ISubscriptionClient> _subscriptionClientMock;

        private AzureServiceBusTopicEventReceiver _azureServiceBusTopicEventReceiver;
        private Func<Message, CancellationToken, Task> _messageHandler;
        private MessageHandlerOptions _messageHandlerOptions;

        [SetUp]
        public async Task SetUp()
        {
            _config = new AzureServiceBusTopicEventReceiverConfig
            {
                ReceiveConnectionString = ReceiveConnectionString,
                ManagementConnectionString = ManagementConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.FromDays(1),
                TopicPath = TopicPath,
                SubscriptionNameGenerator = () => SubscriptionName,
                MaxConcurrentMessages = 10
            };
            _loggerMock = new Mock<ILogger<AzureServiceBusTopicEventReceiver>>(MockBehavior.Strict);
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _topicSubscriptionsServiceMock = new Mock<ITopicSubscriptionsService>(MockBehavior.Strict);
            _subscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>(MockBehavior.Strict);
            _subscriptionClientMock = new Mock<ISubscriptionClient>(MockBehavior.Strict);

            _azureServiceBusTopicEventReceiver = new AzureServiceBusTopicEventReceiver(
                _loggerMock.Object,
                Options.Create(_config),
                _publishingServiceMock.Object,
                _eventsSerializationServiceMock.Object,
                _topicSubscriptionsServiceMock.Object,
                _subscriptionClientFactoryMock.Object
            );

            var cts = new CancellationTokenSource();

            _topicSubscriptionsServiceMock
                .Setup(x => x.CreateSubscriptionAsync(
                    _config.ManagementConnectionString,
                    SubscriptionName,
                    _config.TopicPath,
                    _config.SubscriptionsAutoDeleteOnIdleTimeout,
                    cts.Token
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _subscriptionClientFactoryMock
                .Setup(x => x.GetNew(_config.ReceiveConnectionString, SubscriptionName))
                .Returns(_subscriptionClientMock.Object)
                .Verifiable();

            _messageHandler = null;
            _messageHandlerOptions = null;

            _subscriptionClientMock
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

            await _azureServiceBusTopicEventReceiver.StartReceivingAsync(cts.Token);
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _publishingServiceMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _topicSubscriptionsServiceMock.Verify();
            _subscriptionClientFactoryMock.Verify();
            _subscriptionClientMock.Verify();
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

            _subscriptionClientMock
                .Setup(x => x.CompleteAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _messageHandler(new Message(messageBytes), CancellationToken.None);
        }

        [Test]
        public void MessageHandler_ShouldLogExceptions()
        {
            var messageBytes = new byte[5];

            var exception = new Exception();
            _eventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Throws(exception)
                .Verifiable();

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    LoggerMessages.EventIds.ServiceBusExceptionReceived,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            _messageHandler(new Message(messageBytes), CancellationToken.None);
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
    }
}
