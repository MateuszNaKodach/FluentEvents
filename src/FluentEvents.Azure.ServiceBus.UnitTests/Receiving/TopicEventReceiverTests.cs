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
    public class TopicEventReceiverTests
    {
        private const string TopicPath = "TopicPath";
        private const string ManagementConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=management;SharedAccessKey=0;EntityPath=0";
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";
        private const string SubscriptionName = "SubscriptionName";

        private AzureTopicEventReceiverConfig m_Config;

        private Mock<ILogger<AzureTopicEventReceiver>> m_LoggerMock;
        private Mock<IPublishingService> m_PublishingServiceMock;
        private Mock<IEventsSerializationService> m_EventsSerializationServiceMock;
        private Mock<ITopicSubscriptionsService> m_TopicSubscriptionsServiceMock;
        private Mock<ISubscriptionClientFactory> m_SubscriptionClientFactoryMock;

        private AzureTopicEventReceiver m_AzureTopicEventReceiver;
        private Mock<ISubscriptionClient> m_SubscriptionClientMock;
        private Func<Message, CancellationToken, Task> m_MessageHandler;
        private MessageHandlerOptions m_MessageHandlerOptions;

        [SetUp]
        public async Task SetUp()
        {
            m_Config = new AzureTopicEventReceiverConfig
            {
                ReceiveConnectionString = ReceiveConnectionString,
                ManagementConnectionString = ManagementConnectionString,
                SubscriptionsAutoDeleteOnIdleTimeout = TimeSpan.FromDays(1),
                TopicPath = TopicPath,
                SubscriptionNameGenerator = () => SubscriptionName,
                MaxConcurrentMessages = 10
            };
            m_LoggerMock = new Mock<ILogger<AzureTopicEventReceiver>>(MockBehavior.Strict);
            m_PublishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            m_EventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            m_TopicSubscriptionsServiceMock = new Mock<ITopicSubscriptionsService>(MockBehavior.Strict);
            m_SubscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>(MockBehavior.Strict);
            m_SubscriptionClientMock = new Mock<ISubscriptionClient>(MockBehavior.Strict);

            m_AzureTopicEventReceiver = new AzureTopicEventReceiver(
                m_LoggerMock.Object,
                Options.Create(m_Config),
                m_PublishingServiceMock.Object,
                m_EventsSerializationServiceMock.Object,
                m_TopicSubscriptionsServiceMock.Object,
                m_SubscriptionClientFactoryMock.Object
            );

            var cts = new CancellationTokenSource();

            m_TopicSubscriptionsServiceMock
                .Setup(x => x.CreateSubscriptionAsync(
                    m_Config.ManagementConnectionString,
                    SubscriptionName,
                    m_Config.TopicPath,
                    m_Config.SubscriptionsAutoDeleteOnIdleTimeout,
                    cts.Token
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_SubscriptionClientFactoryMock
                .Setup(x => x.GetNew(m_Config.ReceiveConnectionString, SubscriptionName))
                .Returns(m_SubscriptionClientMock.Object)
                .Verifiable();

            m_MessageHandler = null;
            m_MessageHandlerOptions = null;

            m_SubscriptionClientMock
                .Setup(x => x.RegisterMessageHandler(
                    It.IsAny<Func<Message, CancellationToken, Task>>(),
                    It.Is<MessageHandlerOptions>(y => y.MaxConcurrentCalls == m_Config.MaxConcurrentMessages)
                ))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((x, y) =>
                {
                    m_MessageHandler = x;
                    m_MessageHandlerOptions = y;
                })
                .Verifiable();

            await m_AzureTopicEventReceiver.StartReceivingAsync(cts.Token);
        }

        [TearDown]
        public void TearDown()
        {
            m_SubscriptionClientMock.Verify();
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

            m_EventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Returns(pipelineEvent)
                .Verifiable();

            m_PublishingServiceMock
                .Setup(x => x.PublishEventToGlobalSubscriptionsAsync(pipelineEvent))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_MessageHandler(new Message(messageBytes), CancellationToken.None);
        }

        [Test]
        public void MessageHandler_ShouldLogExceptions()
        {
            var messageBytes = new byte[5];

            var exception = new Exception();
            m_EventsSerializationServiceMock
                .Setup(x => x.DeserializeEvent(It.Is<byte[]>(y => y.SequenceEqual(messageBytes))))
                .Throws(exception)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    LoggerMessages.EventIds.ServiceBusExceptionReceived,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            m_MessageHandler(new Message(messageBytes), CancellationToken.None);
        }


        [Test]
        public void ExceptionReceivedHandler_ShouldDeserializeAndPublishEventToGlobalSubscriptions()
        {
            var exceptionReceivedEventArgs = new ExceptionReceivedEventArgs(new Exception(), "", "", "", "");

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    LoggerMessages.EventIds.ServiceBusExceptionReceived,
                    It.IsAny<object>(),
                    exceptionReceivedEventArgs.Exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            m_MessageHandlerOptions.ExceptionReceivedHandler(exceptionReceivedEventArgs);
        }
    }
}
