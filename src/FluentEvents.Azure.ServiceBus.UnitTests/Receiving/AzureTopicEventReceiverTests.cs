using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Receiving;
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

        private AzureTopicEventReceiverOptions _options;

        private Mock<ILogger<AzureTopicEventReceiver>> _loggerMock;
        private Mock<IPublishingService> _publishingServiceMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<ITopicSubscriptionsService> _topicSubscriptionsServiceMock;
        private Mock<ISubscriptionClientFactory> _subscriptionClientFactoryMock;
        private Mock<ISubscriptionClient> _subscriptionClientMock;

        private AzureTopicEventReceiver _azureTopicEventReceiver;
        private Func<Message, CancellationToken, Task> _messageHandler;
        private MessageHandlerOptions _messageHandlerOptions;

        [SetUp]
        public void SetUp()
        {
            _options = new AzureTopicEventReceiverOptions
            {
                ReceiveConnectionString = ReceiveConnectionString,
                ManagementConnectionString = ManagementConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.FromDays(1),
                TopicPath = TopicPath,
                SubscriptionNameGenerator = () => SubscriptionName,
                MaxConcurrentMessages = 10
            };
            _loggerMock = new Mock<ILogger<AzureTopicEventReceiver>>(MockBehavior.Strict);
            _publishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _topicSubscriptionsServiceMock = new Mock<ITopicSubscriptionsService>(MockBehavior.Strict);
            _subscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>(MockBehavior.Strict);
            _subscriptionClientMock = new Mock<ISubscriptionClient>(MockBehavior.Strict);

            _azureTopicEventReceiver = new AzureTopicEventReceiver(
                _loggerMock.Object,
                Options.Create(_options),
                _publishingServiceMock.Object,
                _eventsSerializationServiceMock.Object,
                _topicSubscriptionsServiceMock.Object,
                _subscriptionClientFactoryMock.Object
            );


            _subscriptionClientFactoryMock
                .Setup(x => x.GetNew(_options.ReceiveConnectionString, SubscriptionName))
                .Returns(_subscriptionClientMock.Object)
                .Verifiable();

            _messageHandler = null;
            _messageHandlerOptions = null;

            _subscriptionClientMock
                .Setup(x => x.RegisterMessageHandler(
                    It.IsAny<Func<Message, CancellationToken, Task>>(),
                    It.Is<MessageHandlerOptions>(y => y.MaxConcurrentCalls == _options.MaxConcurrentMessages)
                ))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((x, y) =>
                {
                    _messageHandler = x;
                    _messageHandlerOptions = y;
                })
                .Verifiable();
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

        private void SetUpSubscriptionCreation()
        {
            _topicSubscriptionsServiceMock
                .Setup(x => x.CreateSubscriptionAsync(
                    _options.ManagementConnectionString,
                    SubscriptionName,
                    _options.TopicPath,
                    _options.SubscriptionsAutoDeleteOnIdleTimeout,
                    CancellationToken.None
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        [Test]
        public async Task StartReceivingAsync_WithSubscriptionCreationEnabled_ShouldCreateSubscription()
        {
            _options.IsSubscriptionCreationEnabled = true;

            SetUpSubscriptionCreation();

            await _azureTopicEventReceiver.StartReceivingAsync(CancellationToken.None);
        }

        [Test]
        public async Task StartReceivingAsync_WithSubscriptionCreationDisabled_ShouldNotCreateSubscription()
        {
            _options.IsSubscriptionCreationEnabled = false;

            await _azureTopicEventReceiver.StartReceivingAsync(CancellationToken.None);
        }

        [Test]
        public async Task MessageHandler_ShouldDeserializeAndPublishEventToGlobalSubscriptions()
        {
            await _azureTopicEventReceiver.StartReceivingAsync(CancellationToken.None);
            
            var messageBytes = new byte[] { 1, 2, 3, 4, 5 };
            var pipelineEvent = new PipelineEvent(typeof(object));

            _eventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Returns(pipelineEvent)
                .Verifiable();

            _publishingServiceMock
                .Setup(x => x.PublishEventToGlobalSubscriptionsAsync(pipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _messageHandler(new Message(messageBytes), CancellationToken.None);
        }

        [Test]
        public async Task MessageHandler_ShouldLogExceptions()
        {
            await _azureTopicEventReceiver.StartReceivingAsync(CancellationToken.None);

            var messageBytes = new byte[5];

            var exception = new TestException();
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
                    LoggerMessages.EventIds.MessagesProcessingThrew,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            Assert.That(async () =>
            {
                await _messageHandler(new Message(messageBytes), CancellationToken.None);
            }, Throws.TypeOf<TestException>());
        }
        
        [Test]
        public async Task ExceptionReceivedHandler_ShouldDeserializeAndPublishEventToGlobalSubscriptions()
        {
            await _azureTopicEventReceiver.StartReceivingAsync(CancellationToken.None);

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

            await _messageHandlerOptions.ExceptionReceivedHandler(exceptionReceivedEventArgs);
        }

        private class TestException : Exception
        {
        }
    }
}
