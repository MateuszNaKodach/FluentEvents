using System;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class AzureTopicEventSenderTests
    {
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private Mock<ILogger<AzureTopicEventSender>> m_LoggerMock;
        private Mock<IEventsSerializationService> m_EventsSerializationServiceMock;
        private Mock<ITopicClientFactory> m_TopicClientFactoryMock;
        private AzureTopicEventSender m_AzureTopicEventSender;
        private PipelineEvent m_PipelineEvent;
        private Mock<ITopicClient> m_TopicClientMock;

        [SetUp]
        public void SetUp()
        {
            m_PipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            m_TopicClientMock = new Mock<ITopicClient>(MockBehavior.Strict);
            m_LoggerMock = new Mock<ILogger<AzureTopicEventSender>>(MockBehavior.Strict);
            m_EventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            m_TopicClientFactoryMock = new Mock<ITopicClientFactory>(MockBehavior.Strict);
            m_TopicClientFactoryMock
                .Setup(x => x.GetNew(ValidConnectionString))
                .Returns(m_TopicClientMock.Object)
                .Verifiable();

            m_AzureTopicEventSender = new AzureTopicEventSender(
                m_LoggerMock.Object,
                Options.Create(new AzureTopicEventSenderConfig
                {
                    ConnectionString = ValidConnectionString
                }),
                m_EventsSerializationServiceMock.Object,
                m_TopicClientFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_TopicClientMock.Verify();
            m_LoggerMock.Verify();
            m_EventsSerializationServiceMock.Verify();
            m_TopicClientFactoryMock.Verify();
        }

        [Test]
        public async Task SendAsync_ShouldSendMessage()
        {
            var serializedEventBytes = new byte[] {1, 2, 3, 4, 5};

            m_EventsSerializationServiceMock
                .Setup(x => x.SerializeEvent(m_PipelineEvent))
                .Returns(serializedEventBytes)
                .Verifiable();

            m_TopicClientMock
                .Setup(x => x.SendAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    LoggerMessages.EventIds.MessageSent,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await m_AzureTopicEventSender.SendAsync(m_PipelineEvent);
        }
    }
}
