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
        private Mock<ILogger<AzureTopicEventSender>> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<ITopicClientFactory> _topicClientFactoryMock;
        private AzureTopicEventSender _azureTopicEventSender;
        private PipelineEvent _pipelineEvent;
        private Mock<ITopicClient> _topicClientMock;

        [SetUp]
        public void SetUp()
        {
            _pipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            _topicClientMock = new Mock<ITopicClient>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<AzureTopicEventSender>>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _topicClientFactoryMock = new Mock<ITopicClientFactory>(MockBehavior.Strict);
            _topicClientFactoryMock
                .Setup(x => x.GetNew(Constants.ValidConnectionString))
                .Returns(_topicClientMock.Object)
                .Verifiable();

            _azureTopicEventSender = new AzureTopicEventSender(
                _loggerMock.Object,
                Options.Create(new AzureTopicEventSenderConfig
                {
                    ConnectionString = Constants.ValidConnectionString
                }),
                _eventsSerializationServiceMock.Object,
                _topicClientFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _topicClientMock.Verify();
            _loggerMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _topicClientFactoryMock.Verify();
        }

        [Test]
        public async Task SendAsync_ShouldSendMessage()
        {
            var serializedEventBytes = new byte[] {1, 2, 3, 4, 5};

            _eventsSerializationServiceMock
                .Setup(x => x.SerializeEvent(_pipelineEvent))
                .Returns(serializedEventBytes)
                .Verifiable();

            _topicClientMock
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

            await _azureTopicEventSender.SendAsync(_pipelineEvent);
        }

        [Test]
        public void Dispose_ShouldCloseTopicClient()
        {
            _topicClientMock
                .Setup(x => x.CloseAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _azureTopicEventSender.Dispose();
        }
    }
}
