using System;
using System.Threading.Tasks;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Azure.ServiceBus.Queues.Sending;
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
    public class AzureServiceBusQueueEventSenderTests
    {
        private Mock<ILogger<AzureServiceBusQueueEventSender>> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<IQueueClientFactory> _queueClientFactoryMock;
        private AzureServiceBusQueueEventSender _azureServiceBusTopicEventSender;
        private PipelineEvent _pipelineEvent;
        private Mock<IQueueClient> _queueClientMock;

        [SetUp]
        public void SetUp()
        {
            _pipelineEvent = new PipelineEvent(typeof(object), "", new object(), new object());
            _queueClientMock = new Mock<IQueueClient>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<AzureServiceBusQueueEventSender>>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _queueClientFactoryMock = new Mock<IQueueClientFactory>(MockBehavior.Strict);
            _queueClientFactoryMock
                .Setup(x => x.GetNew(Constants.ValidConnectionString))
                .Returns(_queueClientMock.Object)
                .Verifiable();

            _azureServiceBusTopicEventSender = new AzureServiceBusQueueEventSender(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                Options.Create(new AzureServiceBusQueueEventSenderConfig
                {
                    SendConnectionString = Constants.ValidConnectionString
                }),
                _queueClientFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _queueClientMock.Verify();
            _loggerMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _queueClientFactoryMock.Verify();
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

            await _azureServiceBusTopicEventSender.SendAsync(_pipelineEvent);
        }

        [Test]
        public void Dispose_ShouldCloseTopicClient()
        {
            _queueClientMock
                .Setup(x => x.CloseAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _azureServiceBusTopicEventSender.Dispose();
        }
    }
}
